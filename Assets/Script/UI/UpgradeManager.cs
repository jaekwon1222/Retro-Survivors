using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;
using System.Reflection;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance { get; private set; }

    // --- UI Refs ---
    public GameObject panel;              // UpgradePanel root (set inactive by default)
    public Button[] optionButtons;        // 3 option buttons
    public TMP_Text[] optionTexts; // optional; if null or element missing, we auto-find from button

    // --- Apply targets ---
    public PlayerAutoFire autoFire;       // player shooting
    public PlayerMovement movement;       // player movement
    public UIManager ui;                  // hearts/hp

    Action _onChosen;                     // callback from WaveManager
    System.Random _rng = new System.Random();

    class UpgradeOption
    {
        public string title;
        public Action apply;
        public float weight;
        public UpgradeOption(string t, Action a, float w)
        {
            title = t; apply = a; weight = w;
        }
    }

    List<UpgradeOption> allOptions;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        if (panel) panel.SetActive(false);

        // Initialize all weighted upgrade options
        allOptions = new List<UpgradeOption>
        {
            new UpgradeOption("Power +1", () => { if (autoFire) autoFire.AddDamage(1); }, 23f),
            new UpgradeOption("Move Speed +5%", () => { if (movement) movement.AddSpeedMultiplier(1.05f); }, 23f),
            new UpgradeOption("Heal +1 Heart", () => { if (ui) ui.Heal(1); }, 23f),

            new UpgradeOption("Projectiles +1", () => { IncreaseProjectilesSafe(1); }, 10f),
            new UpgradeOption("Full Heal", () => { if (ui) ui.SetHP(ui.maxHP); }, 10f),
            new UpgradeOption("Big Hit Radius +0.2", () => { if (autoFire) autoFire.AddHitRadius(0.2f); }, 10f)
        };
    }

    // Called by WaveManager with wave index
    public void OpenForWave(int wave, Action onChosen)
    {
        _onChosen = onChosen;
        bool grand = (wave % 5 == 0);
        OpenUpgrade(grand);
    }

    // Called by WaveManager which already knows whether this is a grand upgrade
    public void OpenUpgrade(bool grand)
    {
        var choices = BuildWeightedChoices();
        Show(choices);
    }

    // ---------- choice model ----------
    struct Choice
    {
        public string title;
        public Action apply;
        public Choice(string t, Action a) { title = t; apply = a; }
    }

    List<Choice> BuildWeightedChoices()
    {
        var result = new List<Choice>();
        if (allOptions == null || allOptions.Count == 0) return result;

        float totalWeight = 0f;
        foreach (var opt in allOptions) totalWeight += opt.weight;

        for (int i = 0; i < 3; i++)
        {
            float roll = UnityEngine.Random.Range(0f, totalWeight);
            float sum = 0f;
            foreach (var opt in allOptions)
            {
                sum += opt.weight;
                if (roll <= sum)
                {
                    result.Add(new Choice(opt.title, opt.apply));
                    break;
                }
            }
        }
        return result;
    }

    void Show(List<Choice> choices)
    {
        if (!panel) { Debug.LogError("[UpgradeManager] Panel is missing"); return; }

        // Pause game and enable cursor for UI interaction
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        panel.SetActive(true);

        for (int i = 0; i < optionButtons.Length; i++)
        {
            int idx = i;
            if (i < choices.Count)
            {
                optionButtons[i].gameObject.SetActive(true);

                // Resolve label: prefer provided array entry; otherwise auto-find
                TMP_Text label = null;
                if (optionTexts != null && i < optionTexts.Length && optionTexts[i] != null)
                    label = optionTexts[i];
                if (!label)
                    label = optionButtons[i].GetComponentInChildren<TMP_Text>(true);
                if (label)
                {
                    label.text = choices[i].title;
                }
                else
                {
                    // Final fallback to legacy UGUI Text
                    var ugui = optionButtons[i].GetComponentInChildren<UnityEngine.UI.Text>(true);
                    if (ugui) ugui.text = choices[i].title;
                }

                optionButtons[i].onClick.RemoveAllListeners();
                optionButtons[i].onClick.AddListener(() =>
                {
                    choices[idx].apply?.Invoke();
                    CloseAndContinue();
                });
            }
            else
            {
                optionButtons[i].gameObject.SetActive(false);
            }
        }
    }

    void CloseAndContinue()
    {
        if (panel) panel.SetActive(false);
        // Resume game and restore cursor state for gameplay (adjust to your game style)
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked; // if you do not lock the cursor during gameplay, change to None

        _onChosen?.Invoke();
        _onChosen = null;
    }
    void IncreaseProjectilesSafe(int delta)
    {
        if (!autoFire) return;
        // Primary path: call method if it exists in this project
        try
        {
            autoFire.AddProjectiles(delta);
            return;
        }
        catch (Exception) { /* fall back below */ }

        // Fallback: reflectively increment a field named "projectileCount"
        var f = autoFire.GetType().GetField("projectileCount", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (f != null && f.FieldType == typeof(int))
        {
            int cur = (int)f.GetValue(autoFire);
            int next = Mathf.Max(1, cur + delta);
            f.SetValue(autoFire, next);
            Debug.Log($"[UpgradeManager] projectileCount: {cur} -> {next}");
        }
    }
}
