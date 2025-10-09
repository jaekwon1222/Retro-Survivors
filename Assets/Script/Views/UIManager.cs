using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class UIManager : Singleton<UIManager>
{
    [Header("HUD Refs (Scene_Entry)")]
    public Transform heartsParent;     // parent object for heart icons
    public GameObject heartPrefab;     // prefab for heart image
    public TextMeshProUGUI waveText;   // UI text for wave
    public TextMeshProUGUI scoreText;  // UI text for score
    public int maxHP = 10;             // max health points

    private List<Image> hearts = new List<Image>();
    private int currentHP;
    private int currentWave = 1;
    private int currentScore = 0;
    private float waveTimer = 0f;
    private float waveInterval = 10f;  // seconds between waves

    void Start()
    {
        // init HUD when scene starts
        InitHearts(maxHP);
        SetHP(maxHP);
        UpdateWaveText();
        UpdateScoreText(0);
    }

    void Update()
    {
        // test damage (press H)
        if (Input.GetKeyDown(KeyCode.H))
            Damage(1);

        // auto wave increase every interval
        waveTimer += Time.deltaTime;
        if (waveTimer >= waveInterval)
        {
            waveTimer = 0f;
            currentWave++;
            UpdateWaveText();
        }
    }

    // --- HP logic ---

    public void InitHearts(int count)
    {
        // remove old hearts
        foreach (Transform c in heartsParent)
            Destroy(c.gameObject);
        hearts.Clear();

        // create new hearts
        for (int i = 0; i < count; i++)
        {
            GameObject h = Instantiate(heartPrefab, heartsParent);
            hearts.Add(h.GetComponent<Image>());
        }
    }

    public void SetHP(int hp)
    {
        currentHP = Mathf.Clamp(hp, 0, maxHP);

        // show only remaining hearts
        for (int i = 0; i < hearts.Count; i++)
            hearts[i].enabled = i < currentHP;
    }

    public void Damage(int dmg = 1)
    {
        currentHP -= dmg;
        SetHP(currentHP);
        if (currentHP <= 0)
        {
            Debug.Log("Player Dead");
            // TODO: add GameOver UI later
        }
    }

    // --- Wave logic ---

    public void UpdateWaveText()
    {
        if (waveText)
            waveText.text = $"Wave: {currentWave}";
    }

    // --- Score logic ---

    public void UpdateScoreText(int score)
    {
        currentScore = score;
        if (scoreText)
            scoreText.text = $"Score: {currentScore}";
    }

    public void AddScore(int amount)
    {
        currentScore += amount;
        UpdateScoreText(currentScore);
    }

    // --- Button functions ---

    public void OnClickStart()
    {
        // go to Entry scene when Start is pressed
        GameManager.Instance.LoadScene("Scene_Entry");
        GameManager.Instance.SetState(GameState.Playing);
    }

    public void OnClickBackToMenu()
    {
        // return to Main Menu scene
        SceneManager.LoadScene("Scene_MainMenu");
        GameManager.Instance.SetState(GameState.MainMenu);
        Time.timeScale = 1f;
    }

    public void OnClickQuit()
    {
#if UNITY_EDITOR
        // stop Play mode in editor
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // quit app in build
        Application.Quit();
#endif
    }

    protected override void Awake()
    {
        base.Awake();

        // Kill stray UIManager if not in Entry scene
        if (SceneManager.GetActiveScene().name != "Scene_Entry")
        {
            Destroy(gameObject);
            return;
        }
    }

}