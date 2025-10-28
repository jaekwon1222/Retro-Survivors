using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    // assign AudioSources/Clips in inspector later

    private void OnEnable()
    {
        EventBus.Subscribe<ScoreChangedEvent>(OnScore);
        EventBus.Subscribe<PlayerDamagedEvent>(OnHit);
    }
    private void OnDisable()
    {
        EventBus.Unsubscribe<ScoreChangedEvent>(OnScore);
        EventBus.Unsubscribe<PlayerDamagedEvent>(OnHit);
    }

    void OnScore(ScoreChangedEvent e)
    {
        // play sfx if needed
    }

    void OnHit(PlayerDamagedEvent e)
    {
        // play damage sfx
    }
}