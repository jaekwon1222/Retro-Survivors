using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [Header("Clips")]
    public AudioClip menuClip;     // main menu BGM
    public AudioClip gameClip;     // in-game BGM

    [Header("Source")]
    public AudioSource source;     // target AudioSource (2D)

    [Header("Volumes")]
    [Range(0f, 1f)] public float masterVolume = 0.8f;
    [Range(0f, 1f)] public float bgmVolume = 1.0f;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (!source) source = GetComponent<AudioSource>();
        ApplyVolume();

        // pick correct clip for initial scene
        PlayForScene(SceneManager.GetActiveScene().name);
        SceneManager.sceneLoaded += (scene, mode) => PlayForScene(scene.name);
    }

    void ApplyVolume()
    {
        if (source) source.volume = masterVolume * bgmVolume;
    }

    public void SetMasterVolume(float v) { masterVolume = Mathf.Clamp01(v); ApplyVolume(); }
    public void SetBgmVolume(float v)    { bgmVolume    = Mathf.Clamp01(v); ApplyVolume(); }

    void PlayForScene(string sceneName)
    {
        // simple rule: adjust names to your project
        if (sceneName == "Scene_MainMenu")    Play(menuClip);
        else if (sceneName == "Scene_Entry")  Play(gameClip);
        else                                   Play(gameClip); // default
    }

    public void Play(AudioClip clip)
    {
        if (!source || clip == null) return;
        if (source.clip == clip && source.isPlaying) return;
        source.clip = clip;
        source.loop = true;
        source.Play();
    }

    // --- optional crossfade (call FadeTo(menuClip) or FadeTo(gameClip)) ---
    public void FadeTo(AudioClip next, float time = 1.0f)
    {
        if (!source || next == null) return;
        if (source.clip == next) return;
        StopAllCoroutines();
        StartCoroutine(FadeRoutine(next, time));
    }

    System.Collections.IEnumerator FadeRoutine(AudioClip next, float time)
    {
        float t = 0f;
        float startVol = source.volume;
        while (t < time)
        {
            t += Time.unscaledDeltaTime;
            source.volume = Mathf.Lerp(startVol, 0f, t / time);
            yield return null;
        }
        source.clip = next;
        source.Play();
        t = 0f;
        while (t < time)
        {
            t += Time.unscaledDeltaTime;
            source.volume = Mathf.Lerp(0f, startVol, t / time);
            yield return null;
        }
    }

    public void SetVolume(float v)
    {
        if (!source) return;
        source.volume = Mathf.Clamp01(v);
    }

    public float GetVolume() => source ? source.volume : 1f;
}