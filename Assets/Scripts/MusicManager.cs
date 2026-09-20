using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// Persistent background-music player. One instance survives every scene
// load (DontDestroyOnLoad) and picks a clip by scene name, crossfading
// into it instead of hard-cutting. Moving between gameplay levels keeps the
// same clip playing instead of restarting it, since they all resolve to the
// same "gameplayMusic" clip.
//
// It uses one plain 2D AudioSource. Loudness is:
//   clip loudness  x  Music setting (AudioSource.volume)  x  Master setting (AudioListener.volume)
// The music files in Assets/Audio are normalised to about -16 LUFS, so the
// default settings (Master 1.0, Music 0.8) give clearly audible background music.
//
// The instance is created automatically from Resources/MusicManager the
// first time any scene loads, so music also plays when a scene other than
// StartScene is opened directly.
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
    public float fadeDuration = 0.8f;

    private AudioSource source;
    private AudioClip currentClip;
    private Coroutine fadeRoutine;
    private bool fading;

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

    // Music slider moved while a track is already playing: apply it right away.
    private void ApplyVolume()
    {
        if (source != null && !fading)
            source.volume = GameSettings.MusicVolume;
    }

    private void PlayForScene(string sceneName)
    {
        AudioClip target;

        switch (sceneName)
        {
            case "StartScene":
                target = startSceneMusic;
                break;
            case "RooftopScene":
                target = rooftopMusic;
                break;
            case "WinScene":
                target = winMusic;
                break;
            case "GameOverScene":
                target = gameOverMusic;
                break;
            default:
                target = gameplayMusic;
                break;
        }

        if (target == currentClip)
            return;

        currentClip = target;

        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(CrossfadeTo(target));
    }

    private IEnumerator CrossfadeTo(AudioClip clip)
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
        source.loop = true;

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
            source.volume = Mathf.Lerp(0f, GameSettings.MusicVolume, t / fadeDuration);
            yield return null;
        }

        fading = false;
        source.volume = GameSettings.MusicVolume;
    }
}
