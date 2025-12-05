using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;
using System.Reflection;

public class UpgradeManager : MonoBehaviour
{
    [Header("Build Type")]
    public bool meleeBuild = false;

    [Header("Companion Settings")]
    public GameObject saltyPPrefab;

    [Header("Upgrade Icons")]
    public Sprite powerIcon;
    public Sprite moveSpeedIcon;
    public Sprite healIcon;
    public Sprite projectileIcon;
    public Sprite fullHealIcon;
    public Sprite bigRadiusIcon;
    public Sprite pierceIcon;
    public Sprite attackSpeedIcon;
    public Sprite knockbackIcon;
    public Sprite meleedmgIcon;
    public Sprite attackrateIcon;
    public Sprite bleedIcon;

    Dictionary<string, Sprite> upgradeSprites;

    //public static UpgradeManager Instance { get; private set; }

    // --- UI Refs ---
    public GameObject panel;              // UpgradePanel root (set inactive by default)
    public Button[] optionButtons;        // 3 option buttons
    public TMP_Text[] optionTexts; // optional; if null or element missing, we auto-find from button
    public Image[] optionImages;

    // --- Apply targets ---
    public PlayerAutoFire autoFire;       // player shooting
    public PlayerMovement movement;       // player movement
    public UIManager ui;                  // hearts/hp
    public PlayerAimWeapon manualFire;
    public PlayerMeleeWeapon meleeWeapon;

    Action _onChosen;                     // callback from WaveManager
    System.Random _rng = new System.Random();
    int powerLevel = 0;
    const int powerMax = 2;

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
    List<UpgradeOption> meleeOptions;


    void Awake()
    {
        //if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        //Instance = this;
        if (panel) panel.SetActive(false);

        // Initialize all weighted upgrade options
        allOptions = new List<UpgradeOption>
        {
            new UpgradeOption("Power +1", () =>
            {
                powerLevel++;

                if (autoFire) autoFire.AddDamage(1);
                if (manualFire)
                {
                    manualFire.AddDamage(1);
                    Debug.Log("[UpgradeManager] ManualFire: Applied Damage +1");
                }

                if (powerLevel >= powerMax)
                {
                    var opt = allOptions.Find(o => o.title == "Power +1");
                    if (opt != null) allOptions.Remove(opt);
                }
            }, 23f),
            new UpgradeOption("Move Speed +15%", () => { if (movement) movement.AddSpeedMultiplier(1.15f); }, 23f),
            new UpgradeOption("Heal +1 Heart", () => { if (ui) ui.Heal(1); }, 23f),

            new UpgradeOption("Projectiles +1", () =>
            {
                IncreaseProjectilesSafe(1);       // auto
                if (manualFire) manualFire.AddProjectiles(1);
            }, 10f),
            new UpgradeOption("Full Heal", () => { if (ui) ui.SetHP(ui.maxHP); }, 10f),
            //new UpgradeOption("Big Hit Radius +0.2", () =>
            //{
             //   if (autoFire) autoFire.AddHitRadius(0.2f);
             //   if (manualFire) manualFire.AddHitRadius(0.2f);
            //}, 10f),
            new UpgradeOption("Pierce +1", () =>
            {
                if (autoFire) autoFire.AddPierce(1);
                if (manualFire) manualFire.AddPierce(1);
            }, 10f),
            new UpgradeOption("Attack Speed +10%", () =>
            {
                if (autoFire) autoFire.AddFireRateMultiplier(1.10f);
                if (manualFire) manualFire.AddFireRateMultiplier(1.10f);
            }, 10f)
        };

        upgradeSprites = new Dictionary<string, Sprite>
        {
            { "Power +1", powerIcon },
            { "Move Speed +5%", moveSpeedIcon },
            { "Heal +1 Heart", healIcon },
            { "Projectiles +1", projectileIcon },
            { "Full Heal", fullHealIcon },
            { "Big Hit Radius +0.2", bigRadiusIcon },
            { "Pierce +1", pierceIcon },
            { "Attack Speed +10%", attackSpeedIcon },
            { "Knockback +20%", knockbackIcon },
            { "Melee Damage +1", meleedmgIcon },
            { "Attack Rate +10%", attackrateIcon },
            { "Bleed (2 dmg over time)", bleedIcon }
        };

        meleeOptions = new List<UpgradeOption>
        {
            new UpgradeOption("Knockback +20%", () =>
            {
                if (meleeWeapon)
                    meleeWeapon.knockbackMultiplier += 0.20f;

            }, 25f),

            new UpgradeOption("Melee Damage +1", () =>
            {
                if (meleeWeapon)
                    meleeWeapon.damage += 1;

            }, 25f),

            //new UpgradeOption("Attack Rate +10%", () =>
            //{
            //    if (meleeWeapon)
            //        meleeWeapon.attackRateMultiplier *= 1.10f;

            //}, 25f),

            new UpgradeOption("Bleed (2 dmg over time)", () =>
            {
                if (meleeWeapon)
                    meleeWeapon.enableBleed = true;

            }, 25f),

            new UpgradeOption("Move Speed +5%", () => { if (movement) movement.AddSpeedMultiplier(1.05f); }, 23f),
            new UpgradeOption("Heal +1 Heart", () => { if (ui) ui.Heal(1); }, 23f),
            new UpgradeOption("Full Heal", () => { if (ui) ui.SetHP(ui.maxHP); }, 10f)
        };
        Debug.Log("[UpgradeManager] meleeOptions built: " + meleeOptions.Count);


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
        Debug.Log("Upgrades using: " + (meleeBuild ? "MELEE" : "RANGED"));
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
        List<UpgradeOption> pool = meleeBuild ? meleeOptions : allOptions;
        Debug.Log("[UpgradeManager] Selected pool = " + (meleeBuild ? "MELEE" : "RANGED")
          + " | Count = " + pool.Count);
        var result = new List<Choice>();
        if (pool == null || pool.Count == 0) return result;

        float totalWeight = 0f;
        foreach (var opt in pool) totalWeight += opt.weight;

        for (int i = 0; i < 3; i++)
        {
            float roll = UnityEngine.Random.Range(0f, totalWeight);
            float sum = 0f;
            foreach (var opt in pool)
            {
                sum += opt.weight;
                if (roll <= sum)
                {
                    Debug.Log("[UpgradeManager] PICKED: " + opt.title);
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
        SFXManager.Instance?.PlayUpgradeOpen(); // play sfx when upgrade panel opens

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

                if (optionImages != null && i < optionImages.Length && optionImages[i] != null)
                {
                    if (upgradeSprites.TryGetValue(choices[i].title, out Sprite icon))
                    {
                        optionImages[i].sprite = icon;
                        optionImages[i].enabled = true;
                    }
                    else
                    {
                        optionImages[i].enabled = false; // no icon found
                    }
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

    // ---- Endless Mode Entry ----
    public void OpenForEndless(Action onChosen)
    {
        _onChosen = onChosen;
        OpenUpgrade(false);   // no grand upgrades in endless
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

    void Start()
    {
        // --------------------------------------------------------------------
        // Reward_ExtraProjectile (already existed)
        // --------------------------------------------------------------------
        if (PlayerPrefs.GetInt("Reward_ExtraProjectile", 0) == 1)
        {
            Debug.Log("[Bonus] Applying extra projectile from achievement");
            IncreaseProjectilesSafe(1);
        }

        // --------------------------------------------------------------------
        // Reward_PlusPower
        // +1 Melee damage
        // +1 Power (manual + auto fire)
        // --------------------------------------------------------------------
        if (PlayerPrefs.GetInt("Reward_PlusPower", 0) == 1)
        {
            Debug.Log("[Bonus] Reward_PlusPower applied");

            if (meleeWeapon) meleeWeapon.damage += 1;
            if (manualFire) manualFire.AddDamage(1);
            if (autoFire) autoFire.AddDamage(1);
        }

        // --------------------------------------------------------------------
        // Reward_KnockPierce
        // +20% melee knockback
        // +1 pierce for manualfire and autofire
        // --------------------------------------------------------------------
        if (PlayerPrefs.GetInt("Reward_KnockPierce", 0) == 1)
        {
            Debug.Log("[Bonus] Reward_KnockPierce applied");

            if (meleeWeapon) meleeWeapon.knockbackMultiplier += 0.20f;
            if (manualFire) manualFire.AddPierce(1);
            if (autoFire) autoFire.AddPierce(1);
        }

        // --------------------------------------------------------------------
        // Reward_MoveSpeed
        // +15% move speed for both melee and ranged builds
        // --------------------------------------------------------------------
        if (PlayerPrefs.GetInt("Reward_MoveSpeed", 0) == 1)
        {
            Debug.Log("[Bonus] Reward_MoveSpeed applied");

            if (movement) movement.AddSpeedMultiplier(1.15f);
        }

        // --------------------------------------------------------------------
        // Reward_PowerUpv2
        // Same effect as PlusPower
        // --------------------------------------------------------------------
        if (PlayerPrefs.GetInt("Reward_PoweUpv2", 0) == 1)
        {
            Debug.Log("[Bonus] Reward_PoweUpv2 applied");

            if (meleeWeapon) meleeWeapon.damage += 1;
            if (manualFire) manualFire.AddDamage(1);
            if (autoFire) autoFire.AddDamage(1);
        }

        // --------------------------------------------------------------------
        // Reward_MoveSpeedv2
        // +15% move speed again (stacks)
        // --------------------------------------------------------------------
        if (PlayerPrefs.GetInt("Reward_MoveSpeedv2", 0) == 1)
        {
            Debug.Log("[Bonus] Reward_MoveSpeedv2 applied");

            if (movement) movement.AddSpeedMultiplier(1.15f);
        }

        // --------------------------------------------------------------------
        // Reward_KnockPiercev2
        // +20% knockback again (stacks)
        // +1 pierce again
        // --------------------------------------------------------------------
        if (PlayerPrefs.GetInt("Reward_KnockPiercev2", 0) == 1)
        {
            Debug.Log("[Bonus] Reward_KnockPiercev2 applied");

            if (meleeWeapon) meleeWeapon.knockbackMultiplier += 0.20f;
            if (manualFire) manualFire.AddPierce(1);
            if (autoFire) autoFire.AddPierce(1);
        }

        if (PlayerPrefs.GetInt("Reward_Companion", 0) == 1)
        {
            Debug.Log("[Bonus] Reward_Companion applied");

            if (saltyPPrefab != null)
            {
                saltyPPrefab.SetActive(true);   // turns the companion on
            }
            else
            {
                Debug.LogWarning("[Bonus] SaltyP object not assigned!");
            }
        }
    }


}
