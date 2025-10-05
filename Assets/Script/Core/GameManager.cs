using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public GameState State { get; private set; } = GameState.Boot;
    public int Score { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        // init things if needed
        SetState(GameState.MainMenu);
    }

    public void SetState(GameState newState)
    {
        // centralize transitions
        State = newState;
        switch (State)
        {
            case GameState.MainMenu:
                // ensure main menu scene loaded later by UI
                break;
            case GameState.Loading:
                break;
            case GameState.Playing:
                Time.timeScale = 1f;
                break;
            case GameState.Paused:
                Time.timeScale = 0f;
                break;
            case GameState.GameOver:
                Time.timeScale = 0f;
                break;
        }
    }

    public void AddScore(int delta)
    {
        Score += delta;
        EventBus.Publish(new ScoreChangedEvent(Score));
    }

    public void PauseToggle()
    {
        if (State == GameState.Playing) SetState(GameState.Paused);
        else if (State == GameState.Paused) SetState(GameState.Playing);
    }

    public void LoadScene(string sceneName)
    {
        // simple scene load wrapper
        SetState(GameState.Loading);
        SceneManager.LoadScene(sceneName);
    }
}