using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    // hook your HUD refs via inspector later
    // e.g., public TMPro.TextMeshProUGUI scoreText;

    private void OnEnable()
    {
        EventBus.Subscribe<ScoreChangedEvent>(OnScoreChanged);
        EventBus.Subscribe<PlayerDamagedEvent>(OnPlayerDamaged);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<ScoreChangedEvent>(OnScoreChanged);
        EventBus.Unsubscribe<PlayerDamagedEvent>(OnPlayerDamaged);
    }

    private void OnScoreChanged(ScoreChangedEvent e)
    {
        // update HUD
        // if (scoreText) scoreText.text = e.Score.ToString();
    }

    private void OnPlayerDamaged(PlayerDamagedEvent e)
    {
        // update HP bar
    }

    // Menu actions
    public void OnClickStart()
    {
        GameManager.Instance.LoadScene("Scene_Entry");
        GameManager.Instance.SetState(GameState.Playing);
    }
}