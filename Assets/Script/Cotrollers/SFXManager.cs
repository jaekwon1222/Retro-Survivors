using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance { get; private set; }

    [Header("Audio Source")]
    public AudioSource oneShot; // assign the AudioSource on this GameObject

    [Header("Clips")]
    public AudioClip sfxShoot;
    public AudioClip sfxHit;
    public AudioClip sfxUpgradeOpen;
    public AudioClip sfxEnemyDie;
    public AudioClip sfxHeal;
    public AudioClip sfxClick;

    void Awake()
    {
        // Singleton guard
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        // Auto bind if not set
        if (!oneShot) oneShot = GetComponent<AudioSource>();

        // Persist across scene loads
        DontDestroyOnLoad(gameObject);

        // Ensure basic AudioSource configuration for 2D one-shots
        if (oneShot)
        {
            oneShot.playOnAwake = false;
            oneShot.loop = false;
            oneShot.spatialBlend = 0f; // 2D
            if (!oneShot.outputAudioMixerGroup)
            {
                // Keep default output if no mixer is assigned
            }
        }
    }

    void OnValidate()
    {
        // Auto-bind the co-located AudioSource in Editor
        if (!oneShot) oneShot = GetComponent<AudioSource>();
        if (oneShot)
        {
            oneShot.playOnAwake = false;
            oneShot.loop = false;
            oneShot.spatialBlend = 0f;
        }
    }

    // --- One-shot helpers ---
    public void PlayShoot() { Play(sfxShoot); }
    public void PlayHit() { Play(sfxHit); }
    public void PlayUpgradeOpen() { Play(sfxUpgradeOpen); }
    public void PlayEnemyDie() { Play(sfxEnemyDie); }
    public void PlayHeal() { Play(sfxHeal); }
    public void PlayClick() { Play(sfxClick); }

    void Play(AudioClip clip)
    {
        if (!oneShot || !clip) return;
        oneShot.PlayOneShot(clip);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Bootstrap()
    {
        // If no instance exists after a scene load, create one so SFX is always available
        if (Instance == null)
        {
            var go = new GameObject("SFXManager");
            var mgr = go.AddComponent<SFXManager>();
            var src = go.GetComponent<AudioSource>();
            if (!src) src = go.AddComponent<AudioSource>();
            mgr.oneShot = src;
        }
    }
    public void SetVolume(float v)
    {
        if (!oneShot) return;
        oneShot.volume = Mathf.Clamp01(v);
    }

    public float GetVolume() => oneShot ? oneShot.volume : 1f;
}

