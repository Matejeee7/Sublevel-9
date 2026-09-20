using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// Persistent background-music player. One instance survives every scene
// load (DontDestroyOnLoad) and picks a clip by scene name, crossfading
// into it instead of hard-cutting. Menu/gameplay tracks loop; the win
// track does not restart every time the player lingers on WinScene.
// Moving between gameplay levels keeps the same clip playing instead of
// restarting it, since they all resolve to the same "gameplayMusic" clip.
//
// The instance is created automatically from Resources/MusicManager the
// first time any scene loads, so music also plays when a scene other than
// StartScene is opened directly. The loudness heard is
// musicVolume * GameSettings.MusicVolume (the Music slider).
public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [Header("Clips")]
    public AudioClip startSceneMusic;
    public AudioClip gameplayMusic;
    public AudioClip rooftopMusic;
    public AudioClip winMusic;
    public AudioClip gameOverMusic;

    [Header("Mix")]
    [Range(0f, 1f)] public float musicVolume = 0.35f;
    public float fadeDuration = 0.8f;

    // The imported tracks are mastered very quietly (average level around
    // -33 to -45 dBFS, peaks of 0.06 to 0.23), so even at full AudioSource
    // volume they were barely audible. Each track gets a fixed make-up gain
    // (applied in OnAudioFilterRead) that brings it to a comparable
    // background level; musicVolume and the Music slider then scale it.
    [Header("Make-up gain (per track)")]
    public float startSceneGain = 12f;
    public float gameplayGain = 9f;
    public float rooftopGain = 15f;
    public float winGain = 5f;
    public float gameOverGain = 5f;

    private AudioSource source;
    private AudioClip currentClip;
    private Coroutine fadeRoutine;
    private bool fading;
    private volatile float gain = 1f;

    private float TargetVolume => musicVolume * GameSettings.MusicVolume;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Bootstrap()
    {
        if (Instance != null)
            return;

        GameObject prefab = Resources.Load<GameObject>("MusicManager");
        if (prefab != null)
            Instantiate(prefab).name = "MusicManager";
        else
            Debug.LogWarning("MusicManager prefab missing from Resources - no background music.");
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        source = GetComponent<AudioSource>();
        if (source == null)
            source = gameObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.spatialBlend = 0f;
        source.mute = false;
        source.volume = 0f;

        SceneManager.sceneLoaded += HandleSceneLoaded;
        GameSettings.Changed += ApplyVolume;

        EnsureListener();
        PlayForScene(SceneManager.GetActiveScene().name);
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= HandleSceneLoaded;
            GameSettings.Changed -= ApplyVolume;
            Instance = null;
        }
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        EnsureListener();
        PlayForScene(scene.name);
    }

    // Nothing is audible without an AudioListener, and the menu scenes
    // (StartScene, WinScene, GameOverScene) have none on their cameras.
    private static void EnsureListener()
    {
        if (FindFirstObjectByType<AudioListener>() != null)
            return;

        Camera cam = Camera.main != null ? Camera.main : FindFirstObjectByType<Camera>();
        if (cam != null)
            cam.gameObject.AddComponent<AudioListener>();
    }

    // Slider moved while a track is already playing: apply it right away.
    private void ApplyVolume()
    {
        if (source != null && !fading)
            source.volume = TargetVolume;
    }

    private void PlayForScene(string sceneName)
    {
        AudioClip target;
        float targetGain;
        bool loop = true;

        switch (sceneName)
        {
            case "StartScene":
                target = startSceneMusic;
                targetGain = startSceneGain;
                break;
            case "RooftopScene":
                target = rooftopMusic;
                targetGain = rooftopGain;
                break;
            case "WinScene":
                target = winMusic;
                targetGain = winGain;
                break;
            case "GameOverScene":
                target = gameOverMusic;
                targetGain = gameOverGain;
                break;
            default:
                target = gameplayMusic;
                targetGain = gameplayGain;
                break;
        }

        if (target == currentClip)
            return;

        currentClip = target;

        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(CrossfadeTo(target, targetGain, loop));
    }

    private IEnumerator CrossfadeTo(AudioClip clip, float clipGain, bool loop)
    {
        fading = true;

        float startVolume = source.volume;
        float t = 0f;
        while (t < fadeDuration && source.volume > 0f)
        {
            t += Time.unscaledDeltaTime;
            source.volume = Mathf.Lerp(startVolume, 0f, t / fadeDuration);
            yield return null;
        }

        source.volume = 0f;
        source.Stop();
        source.clip = clip;
        source.loop = loop;
        gain = clipGain;

        if (clip == null)
        {
            fading = false;
            yield break;
        }

        source.Play();

        t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            source.volume = Mathf.Lerp(0f, TargetVolume, t / fadeDuration);
            yield return null;
        }

        fading = false;
        source.volume = TargetVolume;
    }

    // Audio thread: apply the per-track make-up gain, with a gentle limiter
    // so the rare loud peak is rounded off instead of clipping.
    void OnAudioFilterRead(float[] data, int channels)
    {
        float g = gain;
        for (int i = 0; i < data.Length; i++)
        {
            float x = data[i] * g;
            float a = x < 0f ? -x : x;
            if (a > 0.7f)
            {
                float limited = 0.7f + 0.3f * (float)System.Math.Tanh((a - 0.7f) / 0.3f);
                x = x < 0f ? -limited : limited;
            }
            data[i] = x;
        }
    }
}
