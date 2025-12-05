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

    [SerializeField] public GameObject gameOverPanel;

    protected override void Awake()
    {
        base.Awake();
        // UIManager should live only in Entry scene
        if (SceneManager.GetActiveScene().name != "Scene_Entry" && SceneManager.GetActiveScene().name != "Scene_Entry 1")
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
        SetWave(1, FindAnyObjectByType<WaveManager>().maxWaves);
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
            ShowGameOver();
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
        //SFXManager.Instance?.PlayHeal();
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
        // resume time
        Time.timeScale = 1f;

        // unlock cursor for menu
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // reset player stats
        var auto = FindObjectOfType<PlayerAutoFire>();
        if (auto) auto.ResetStats();

        var move = FindObjectOfType<PlayerMovement>();
        // if (move) move.ResetStats();

        // destroy UIManager so when returning to Scene_Entry1 we get a fresh one
        Destroy(gameObject);

        // load main menu
        SceneManager.LoadScene("Scene_MainMenu");
    }

    public void OnClickVictoryMainMenu()
    {
        Time.timeScale = 1f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene("Scene_MainMenu");
    }

    public void OnClickVictoryMissionSelect()
    {
        // resume time
        Time.timeScale = 1f;

        // unlock cursor for menu
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // reset player stats
        var auto = FindObjectOfType<PlayerAutoFire>();
        if (auto) auto.ResetStats();

        var move = FindObjectOfType<PlayerMovement>();
        // if (move) move.ResetStats();

        // destroy UIManager so when returning to Scene_Entry1 we get a fresh one
        Destroy(gameObject);
        SceneManager.LoadScene("StoryMenu");
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
        //if (move) move.ResetStats();

        // destroy UIManager so a fresh one is created in the new Scene_Entry
        Destroy(gameObject);

        // reload entry scene
        SceneManager.LoadScene("Scene_Entry 1");
    }

    // ---------------- Wave / Score ----------------
    public void SetWave(int wave, int maxWaves)
    {
        currentWave = Mathf.Max(1, wave);
        if (waveText) waveText.text = $"Wave: {currentWave} / {maxWaves}";
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

    public void SetupHeartsForPlayer(int hp)
    {
        maxHP = hp;
        InitHearts(maxHP);
        SetHP(maxHP);
    }


    // ---------------- Buttons ----------------

    public void OnClickBackToMenu()
    {
        SceneManager.LoadScene("Scene_MainMenu");
        Time.timeScale = 1f;
    }
}