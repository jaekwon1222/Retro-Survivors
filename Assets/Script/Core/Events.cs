// Base event type
public abstract class GameEvent { }

// Sample events
public sealed class ScoreChangedEvent : GameEvent
{
    public int Score;
    public ScoreChangedEvent(int score) { Score = score; }
}

public sealed class PlayerDamagedEvent : GameEvent
{
    public int CurrentHP;
    public PlayerDamagedEvent(int hp) { CurrentHP = hp; }
}