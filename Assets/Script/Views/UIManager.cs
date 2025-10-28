using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class UIManager : Singleton<UIManager>
{
    [Header("HUD Refs (Scene_Entry)")]
    public Transform heartsParent;       // parent for heart icons
    public GameObject heartPrefab;       // prefab for one heart (UI Image)
    public TextMeshProUGUI waveText;     // "Wave: n"
    public TextMeshProUGUI scoreText;    // "Score: n"
    public int maxHP = 10;               // maximum hearts

    private readonly List<Image> hearts = new List<Image>();
    private int currentHP;
    private int currentWave = 1;
    private int currentScore = 0;

    protected override void Awake()
    {
        base.Awake();

        // Live only in Entry scene
        if (SceneManager.GetActiveScene().name != "Scene_Entry")
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // Init HUD
        InitHearts(maxHP);
        SetHP(maxHP);
        SetWave(1);
        SetScore(0);
    }

    // ---------------- HP / Hearts ----------------

    public void InitHearts(int count)
    {
        if (!heartsParent || !heartPrefab) return;

        // Clear old
        foreach (Transform c in heartsParent)
            Destroy(c.gameObject);
        hearts.Clear();

        // Build new
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

        // Toggle heart visibility by remaining HP
        for (int i = 0; i < hearts.Count; i++)
            hearts[i].enabled = (i < currentHP);
    }

    public void Damage(int amount = 1)
    {
        // Debug.Log($"[UIManager] Damage {amount} (before {currentHP})");
        SetHP(currentHP - amount);

        if (currentHP <= 0)
        {
            // TODO: show game over UI
            Debug.Log("[UIManager] Player Dead");
        }
    }

    // Heal by amount (clamped in SetHP)
    public void Heal(int amount = 1)
    {
        SetHP(currentHP + amount);
    }

    // Full heal to max
    public void FullHeal()
    {
        SetHP(maxHP);
    }

    // ---------------- Wave / Score ----------------

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

    // ---------------- Buttons ----------------

    public void OnClickBackToMenu()
    {
        SceneManager.LoadScene("Scene_MainMenu");
        Time.timeScale = 1f;
    }
}