using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class UIManager : Singleton<UIManager>
{
    [Header("HUD Refs (Scene_Entry)")]
    public Transform heartsParent;     // parent for heart icons (TopBar/Hearts)
    public GameObject heartPrefab;     // prefab for a single heart (UI Image)
    public TextMeshProUGUI waveText;   // Wave text (e.g., "Wave: 1")
    public TextMeshProUGUI scoreText;  // Score text (e.g., "Score: 0")
    public int maxHP = 10;             // max health points

    private readonly List<Image> hearts = new List<Image>();
    private int currentHP;
    private int currentWave = 1;
    private int currentScore = 0;

    protected override void Awake()
    {
        base.Awake();
        // UIManager should live only in Entry scene
        if (SceneManager.GetActiveScene().name != "Scene_Entry")
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // init HUD at scene start
        InitHearts(maxHP);
        SetHP(maxHP);
        SetWave(1);
        SetScore(0);
    }

    // -------- HP / Hearts --------
    public void InitHearts(int count)
    {
        if (!heartsParent || !heartPrefab) return;

        foreach (Transform c in heartsParent) Destroy(c.gameObject);
        hearts.Clear();

        for (int i = 0; i < count; i++)
        {
            var go = Instantiate(heartPrefab, heartsParent);
            var img = go.GetComponent<Image>();
            hearts.Add(img);
        }
    }

    public void SetHP(int hp)
    {
        currentHP = Mathf.Clamp(hp, 0, maxHP);

        // show only remaining hearts
        for (int i = 0; i < hearts.Count; i++)
            hearts[i].enabled = i < currentHP;
    }

    public void Damage(int amount = 1)
{
    Debug.Log($"[UIManager] Damage before={currentHP}, after={currentHP - amount}, hearts={hearts.Count}"); // 로그 3
    SetHP(currentHP - amount);
    if (currentHP <= 0)
    {
        Debug.Log("Player Dead");
        // TODO: show game over UI
    }
}

    public void Heal(int amount = 1) => SetHP(currentHP + amount);

    // -------- Wave / Score --------
    public void SetWave(int wave)
    {
        currentWave = Mathf.Max(1, wave);
        if (waveText) waveText.text = $"Wave: {currentWave}";
    }

    public void SetScore(int score)
    {
        currentScore = Mathf.Max(0, score);
        if (scoreText) scoreText.text = $"Score: {currentScore}";
    }

    public void AddScore(int delta)
    {
        SetScore(currentScore + delta);
    }

    // -------- Buttons --------
    public void OnClickBackToMenu()
    {
        SceneManager.LoadScene("Scene_MainMenu");
        Time.timeScale = 1f;
    }
}