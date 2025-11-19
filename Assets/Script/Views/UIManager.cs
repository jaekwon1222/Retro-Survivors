using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : Singleton<UIManager>
{
    [Header("HUD Refs (Scene_Entry)")]
    [SerializeField] public Transform heartsParent;   // parent for heart icons (TopBar/Hearts)
    [SerializeField] public GameObject heartPrefab;   // prefab for a single heart (UI Image)
    [SerializeField] public TextMeshProUGUI waveText; // "Wave: X"
    [SerializeField] public TextMeshProUGUI scoreText;// "Score: Y"
    [SerializeField] public int maxHP = 10;           // max hearts

    [Header("Game Over")]
    [SerializeField] public GameObject gameOverPanel; // panel shown when player dies

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

    private void Start()
    {
        InitHearts(maxHP);
        SetHP(maxHP);
        SetWave(1);
        SetScore(0);

        if (gameOverPanel)
            gameOverPanel.SetActive(false);
    }

    // ---------------- HP / Hearts ----------------
    public void InitHearts(int count)
    {
        if (!heartsParent || !heartPrefab) return;

        foreach (Transform c in heartsParent)
            Destroy(c.gameObject);

        hearts.Clear();

        for (int i = 0; i < count; i++)
        {
            var go = Instantiate(heartPrefab, heartsParent);
            var img = go.GetComponent<Image>();
            if (img) hearts.Add(img);
        }
    }

    public void SetHP(int hp)
    {
        currentHP = Mathf.Clamp(hp, 0, maxHP);

        // safely update heart images (skip destroyed ones)
        for (int i = 0; i < hearts.Count; i++)
        {
            var img = hearts[i];
            if (!img) continue; // image was destroyed with previous scene

            img.enabled = i < currentHP; // show only remaining hearts
        }
    }

    public void Damage(int amount = 1)
    {
        SetHP(currentHP - amount);
        SFXManager.Instance?.PlayHit();

        if (currentHP <= 0)
        {
            Debug.Log("[UIManager] Player Dead");
            ShowGameOver();
        }
    }

    public void Heal(int amount = 1)
    {
        SetHP(currentHP + amount);
        SFXManager.Instance?.PlayHeal();
    }

    public void FullHeal()
    {
        SetHP(maxHP);
        SFXManager.Instance?.PlayHeal();
    }

    // ---------------- Game Over ----------------
    public void ShowGameOver()
    {
        if (gameOverPanel)
            gameOverPanel.SetActive(true);

        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void OnClickGameOverMainMenu()
    {
        Time.timeScale = 1f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene("Scene_MainMenu");
    }

    public void OnClickGameOverRestart()
    {
        // resume time
        Time.timeScale = 1f;

        // lock cursor again for gameplay
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // reset player stats before restarting
        var auto = FindObjectOfType<PlayerAutoFire>();
        if (auto) auto.ResetStats();

        var move = FindObjectOfType<PlayerMovement>();
        if (move) move.ResetStats();

        // destroy UIManager so a fresh one is created in the new Scene_Entry
        Destroy(gameObject);

        // reload entry scene
        SceneManager.LoadScene("Scene_Entry");
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

    public void AddScore(int delta) => SetScore(currentScore + delta);

    // ---------------- Buttons ----------------
    public void OnClickBackToMenu()
    {
        SceneManager.LoadScene("Scene_MainMenu");
        Time.timeScale = 1f;
    }
}