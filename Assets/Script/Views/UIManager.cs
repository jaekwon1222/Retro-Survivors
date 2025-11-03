using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : Singleton<UIManager>
{
    [Header("HUD Refs (Scene_Entry)")]
    [SerializeField] public Transform heartsParent;   // parent for heart icons (TopBar/Hearts)
    [SerializeField] public GameObject heartPrefab;    // prefab for a single heart (UI Image)
    [SerializeField] public TextMeshProUGUI waveText;  // "Wave: X"
    [SerializeField] public TextMeshProUGUI scoreText; // "Score: Y"
    [SerializeField] public int maxHP = 10;            // max hearts

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
    }

    // ---------------- HP / Hearts ----------------
    public void InitHearts(int count)
    {
        if (!heartsParent || !heartPrefab) return;

        foreach (Transform c in heartsParent) Destroy(c.gameObject);
        hearts.Clear();

        for (int i = 0; i < count; i++)
        {
            var go = Object.Instantiate(heartPrefab, heartsParent);
            var img = go.GetComponent<Image>();
            if (img) hearts.Add(img);
        }
    }

    public void SetHP(int hp)
    {
        currentHP = Mathf.Clamp(hp, 0, maxHP);
        for (int i = 0; i < hearts.Count; i++)
            hearts[i].enabled = i < currentHP; // show only remaining hearts
    }

    public void Damage(int amount = 1)
    {
        SetHP(currentHP - amount);
        if (currentHP <= 0)
        {
            Debug.Log("[UIManager] Player Dead");
            // TODO: show game over UI
        }
    }

    public void Heal(int amount = 1) => SetHP(currentHP + amount);

    public void FullHeal() => SetHP(maxHP);

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