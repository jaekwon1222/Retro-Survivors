using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public int currentWave = 1;
    public int score = 0;

    void Start()
    {
        UIManager.Instance.SetWave(currentWave);
        UIManager.Instance.SetScore(score);
    }

    public void StartNextWave() // call when a new wave begins
    {
        currentWave++;
        UIManager.Instance.SetWave(currentWave);
        Debug.Log($"Wave {currentWave} started");
    }

    public void AddScore(int amount)
    {
        score += amount;
        UIManager.Instance.SetScore(score);
    }

     public void DamagePlayer(int amount = 1)
    {
        Debug.Log($"[GameManager] DamagePlayer {amount}");  
        UIManager.Instance.Damage(amount);                  
    }
}