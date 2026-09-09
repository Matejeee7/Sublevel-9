using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// Persistent background-music player. One instance survives every scene
// load (DontDestroyOnLoad) and picks a clip by scene name, crossfading
// into it instead of hard-cutting. Menu/gameplay tracks loop; the win
// track does not restart every time the player lingers on WinScene.
// Moving between gameplay levels keeps the same clip playing instead of
// restarting it, since they all resolve to the same "gameplayMusic" clip.
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
    [Range(0f, 1f)] public float musicVolume = 0.2f;
    public float fadeDuration = 0.8f;

    private AudioSource source;
    private AudioClip currentClip;
    private Coroutine fadeRoutine;

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
        source.volume = 0f;

        SceneManager.sceneLoaded += HandleSceneLoaded;
        PlayForScene(SceneManager.GetActiveScene().name);
    }

    void OnDestroy()
    {
        if (Instance == this)
            SceneManager.sceneLoaded -= HandleSceneLoaded;
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayForScene(scene.name);
    }

    private void PlayForScene(string sceneName)
    {
        AudioClip target;
        bool loop = true;

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
        fadeRoutine = StartCoroutine(CrossfadeTo(target, loop));
    }

    private IEnumerator CrossfadeTo(AudioClip clip, bool loop)
    {
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

        if (clip == null)
            yield break;

        source.Play();

        t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            source.volume = Mathf.Lerp(0f, musicVolume, t / fadeDuration);
            yield return null;
        }
        source.volume = musicVolume;
    }
}
