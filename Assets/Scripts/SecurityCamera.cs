using UnityEngine;

public class SecurityCamera : MonoBehaviour
{
    [Header("Sweep")]
    public float sweepAngle = 55f;
    public float sweepSpeed = 1.15f;

    [Header("Detection")]
    public float viewDistance = 7f;

    [Range(1f, 180f)]
    public float viewAngle = 50f;

    public float detectionTime = 0.25f;

    [Header("Line Of Sight")]
    public LayerMask obstacleMask;

    [Header("References")]
    public Transform player;

    [Header("View Cone")]
    [Range(3, 80)]
    public int coneResolution = 40;

    [Range(0f, 1f)]
    public float coneAlpha = 0.22f;

    private float startRotation;
    private float detectionTimer;

    private PlayerStealth playerStealth;

    private bool cameraDisabled = false;
    private bool playerCurrentlyVisible = false;

    private GameObject coneObject;
    private Mesh coneMesh;
    private MeshRenderer coneRenderer;
    private Material coneMaterial;

    void Start()
    {
        startRotation = transform.eulerAngles.z;

        // Ako Player nije ručno spojen,
        // pokušaj ga pronaći preko Player taga.
        if (player == null)
        {
            GameObject playerObject =
                GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }

        if (player != null)
        {
            playerStealth =
                player.GetComponent<PlayerStealth>();
        }

        CreateViewCone();
        UpdateViewCone();
    }

    void Update()
    {
        if (cameraDisabled)
            return;

        RotateCamera();
        DetectPlayer();
        UpdateViewCone();
    }

    // =====================================================
    // CAMERA ROTATION
    // =====================================================

    private void RotateCamera()
    {
        float angle =
            Mathf.Sin(Time.time * sweepSpeed) * sweepAngle;

        transform.rotation =
            Quaternion.Euler(
                0f,
                0f,
                startRotation + angle
            );
    }

    // =====================================================
    // PLAYER DETECTION
    // =====================================================

    private void DetectPlayer()
    {
        playerCurrentlyVisible = false;

        if (player == null)
        {
            detectionTimer = 0f;
            return;
        }

        // Ako je igrač skriven u lockeru,
        // kamera ga ne može detektirati.
        if (playerStealth != null &&
            playerStealth.isHidden)
        {
            detectionTimer = 0f;
            return;
        }

        Vector2 cameraPosition =
            transform.position;

        Vector2 playerPosition =
            player.position;

        Vector2 directionToPlayer =
            playerPosition - cameraPosition;

        float distanceToPlayer =
            directionToPlayer.magnitude;

        // Player je izvan maksimalnog dometa.
        if (distanceToPlayer > viewDistance)
        {
            detectionTimer = 0f;
            return;
        }

        directionToPlayer.Normalize();

        // Lokalna +X os je smjer gledanja kamere.
        Vector2 cameraForward =
            transform.right;

        float angleToPlayer =
            Vector2.Angle(
                cameraForward,
                directionToPlayer
            );

        // Player nije unutar vidnog kuta.
        if (angleToPlayer > viewAngle / 2f)
        {
            detectionTimer = 0f;
            return;
        }

        // Provjera zida / prepreke između kamere i playera.
        float obstacleDistance =
            GetDistanceToObstacle(
                cameraPosition,
                directionToPlayer
            );

        if (obstacleDistance < distanceToPlayer)
        {
            detectionTimer = 0f;
            return;
        }

        // Player je stvarno vidljiv.
        playerCurrentlyVisible = true;

        detectionTimer += Time.deltaTime;

        if (detectionTimer >= detectionTime)
        {
            PlayerDetected();
            detectionTimer = 0f;
        }
    }

    private void PlayerDetected()
    {
        Debug.Log(
            "SECURITY CAMERA DETECTED PLAYER! Alerting nearby security."
        );

        EnemyAI[] enemies =
            FindObjectsByType<EnemyAI>(FindObjectsSortMode.None);

        foreach (EnemyAI enemy in enemies)
        {
            enemy.Alert(player.position);
        }
    }

    // =====================================================
    // OBSTACLE DETECTION
    // =====================================================

    private float GetDistanceToObstacle(
        Vector2 origin,
        Vector2 direction
    )
    {
        RaycastHit2D[] hits =
            Physics2D.RaycastAll(
                origin,
                direction,
                viewDistance,
                obstacleMask
            );

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider == null)
                continue;

            // Triggeri ne blokiraju pogled.
            if (hit.collider.isTrigger)
                continue;

            // Sama kamera ne blokira svoj pogled.
            if (hit.collider.transform == transform)
                continue;

            // Child objekti kamere također se ignoriraju.
            if (hit.collider.transform.IsChildOf(transform))
                continue;

            // Ignoriraj praktički nulti hit.
            if (hit.distance < 0.05f)
                continue;

            return hit.distance;
        }

        return viewDistance;
    }

    // =====================================================
    // CAMERA CONTROL TERMINAL
    // =====================================================

    public void DisableCamera()
    {
        cameraDisabled = true;

        if (coneObject != null)
        {
            coneObject.SetActive(false);
        }

        enabled = false;
    }

    // =====================================================
    // CREATE VISIBLE GAME VIEW CONE
    // =====================================================

    private void CreateViewCone()
    {
        coneObject =
            new GameObject("Camera_ViewCone");

        coneObject.transform.SetParent(transform);

        coneObject.transform.localPosition =
            Vector3.zero;

        coneObject.transform.localRotation =
            Quaternion.identity;

        // VAŽNO:
        // Kamera kod tebe ima Scale npr. 0.65.
        // Child bi se inače također smanjio.
        // Ovim poništavamo parent scale samo za cone.
        Vector3 parentScale =
            transform.lossyScale;

        coneObject.transform.localScale =
            new Vector3(
                Mathf.Abs(parentScale.x) > 0.0001f
                    ? 1f / Mathf.Abs(parentScale.x)
                    : 1f,

                Mathf.Abs(parentScale.y) > 0.0001f
                    ? 1f / Mathf.Abs(parentScale.y)
                    : 1f,

                1f
            );

        MeshFilter meshFilter =
            coneObject.AddComponent<MeshFilter>();

        coneRenderer =
            coneObject.AddComponent<MeshRenderer>();

        coneMesh =
            new Mesh();

        coneMesh.name =
            "Security Camera View Cone";

        meshFilter.mesh =
            coneMesh;

        Shader shader =
            Shader.Find("Sprites/Default");

        coneMaterial =
            new Material(shader);

        coneMaterial.color =
            new Color(
                1f,
                0.75f,
                0f,
                coneAlpha
            );

        coneRenderer.material =
            coneMaterial;

        coneRenderer.sortingLayerName =
            "Default";

        coneRenderer.sortingOrder =
            10;
    }

    // =====================================================
    // UPDATE VISIBLE CONE
    // =====================================================

    private void UpdateViewCone()
    {
        if (coneMesh == null)
            return;

        int vertexCount =
            coneResolution + 2;

        Vector3[] vertices =
            new Vector3[vertexCount];

        int[] triangles =
            new int[coneResolution * 3];

        // Prvi vertex je sama kamera.
        vertices[0] =
            Vector3.zero;

        float halfAngle =
            viewAngle / 2f;

        for (int i = 0;
             i <= coneResolution;
             i++)
        {
            float t =
                (float)i / coneResolution;

            float localAngle =
                Mathf.Lerp(
                    -halfAngle,
                    halfAngle,
                    t
                );

            float radians =
                localAngle *
                Mathf.Deg2Rad;

            // Lokalni smjer zrake.
            Vector2 localDirection =
                new Vector2(
                    Mathf.Cos(radians),
                    Mathf.Sin(radians)
                );

            // Pretvaranje u world space za Physics2D.
            Vector2 worldDirection =
                transform.TransformDirection(
                    localDirection
                );

            Vector2 cameraPosition =
                transform.position;

            // Zraka ide do zida ili do View Distance.
            float finalDistance =
                GetDistanceToObstacle(
                    cameraPosition,
                    worldDirection
                );

            // Mesh je u lokalnom prostoru kamere.
            vertices[i + 1] =
                new Vector3(
                    localDirection.x *
                    finalDistance,

                    localDirection.y *
                    finalDistance,

                    0f
                );
        }

        for (int i = 0;
             i < coneResolution;
             i++)
        {
            int triangleIndex =
                i * 3;

            triangles[triangleIndex] =
                0;

            triangles[triangleIndex + 1] =
                i + 1;

            triangles[triangleIndex + 2] =
                i + 2;
        }

        coneMesh.Clear();

        coneMesh.vertices =
            vertices;

        coneMesh.triangles =
            triangles;

        coneMesh.RecalculateBounds();

        // Žuto = kamera traži.
        // Crveno = kamera trenutno vidi playera.
        if (coneMaterial != null)
        {
            if (playerCurrentlyVisible)
            {
                coneMaterial.color =
                    new Color(
                        1f,
                        0.1f,
                        0.1f,
                        Mathf.Max(
                            coneAlpha,
                            0.35f
                        )
                    );
            }
            else
            {
                coneMaterial.color =
                    new Color(
                        1f,
                        0.75f,
                        0f,
                        coneAlpha
                    );
            }
        }
    }

    // =====================================================
    // CLEANUP
    // =====================================================

    private void OnDestroy()
    {
        if (coneMaterial != null)
        {
            Destroy(coneMaterial);
        }

        if (coneMesh != null)
        {
            Destroy(coneMesh);
        }
    }
}