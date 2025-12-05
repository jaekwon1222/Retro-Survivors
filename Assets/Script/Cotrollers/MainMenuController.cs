using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject settingsPanel;

    // Start → load play scene
    public void OnClickStart()
    {
        SceneManager.LoadScene("Scene_Entry 1");
    }

    public void OnClickStoryMode()
    {
        SceneManager.LoadScene("StoryMenu");
    }

    public void OnClickEndlessMode()
    {
        SceneManager.LoadScene("EndlessMenu");
    }

    void Start()
    {
        // Achievements
        PlayerPrefs.SetInt("Achievement_5Kills", 0);
        PlayerPrefs.SetInt("Achievement_25Kills", 0);
        PlayerPrefs.SetInt("Achievement_50Kills", 0);
        PlayerPrefs.SetInt("Achievement_100Kills", 0);
        PlayerPrefs.SetInt("Achievement_150Kills", 0);
        PlayerPrefs.SetInt("Achievement_200Kills", 0);
        PlayerPrefs.SetInt("SaltyP_Pressed", 0); // companion achievement

        // Rewards
        PlayerPrefs.SetInt("Reward_ExtraProjectile", 0);
        PlayerPrefs.SetInt("Reward_PlusPower", 0);
        PlayerPrefs.SetInt("Reward_KnockPierce", 0);
        PlayerPrefs.SetInt("Reward_MoveSpeed", 0);
        PlayerPrefs.SetInt("Reward_PoweUpv2", 0);
        PlayerPrefs.SetInt("Reward_MoveSpeedv2", 0);
        PlayerPrefs.SetInt("Reward_KnockPiercev2", 0);
        PlayerPrefs.SetInt("Reward_Companion", 0);

        // Other stats
        PlayerPrefs.SetInt("TotalKills", 0);

        PlayerPrefs.Save();
        Debug.Log("[Stats] All achievements and rewards have been reset!");
    }


    public void OnClickAchievements()
    {
        SceneManager.LoadScene("Achievements_Scene");
    }

    public void OnClickBack()
    {
        SceneManager.LoadScene("Scene_MainMenu");
    }

    public void OnClickOpenSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);   // turn ON the settings panel
        }
    }

    public void OnClickOutOfSettings()
    {
        if (settingsPanel != null)
        {
            // Toggle active state
            settingsPanel.SetActive(!settingsPanel.activeSelf);
        }
    }

    public void OnClickBackFromStoryMenu()
    {
        SceneManager.LoadScene("GameSelection");
    }

    public void OnClickToModeSelect()
    {
        SceneManager.LoadScene("GameSelection");
    }

    public void OnClickQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // === Settings Panel ===
    public void OnClickSetting()
    {
        Debug.Log("[MainMenu] OnClickSetting clicked");
        ShowPanel(settingsPanel, true);
    }

    public void OnClickCloseSetting()
    {
        ShowPanel(settingsPanel, false);
    }

    void ShowPanel(GameObject p, bool show)
    {
        if (!p)
        {
            Debug.LogWarning("[MainMenu] ShowPanel called with null GameObject");
            return;
        }

        // Ensure object is active so we can apply alpha/interactable
        if (!p.activeSelf && show) p.SetActive(true);

        if (p == settingsPanel && settingsCG != null)
        {
            settingsCG.alpha = show ? 1f : 0f;
            settingsCG.interactable = show;
            settingsCG.blocksRaycasts = show;
            if (!show) p.SetActive(false);
        }
        else
        {
            // Fallback when CanvasGroup is not present
            p.SetActive(show);
        }

        Debug.Log($"[MainMenu] ShowPanel({p.name}, show={show}) | activeSelf={p.activeSelf} | cg={(settingsCG != null ? settingsCG.alpha.ToString("0.00") : "none")}");
    }

    [ContextMenu("DEBUG/Open Settings")]
    void DebugOpen() => ShowPanel(settingsPanel, true);

    [ContextMenu("DEBUG/Close Settings")]
    void DebugClose() => ShowPanel(settingsPanel, false);

    // === Stats Utilities ===
    public void OnClickClearStats()
    {
        PlayerPrefs.SetInt("Achievement_5Kills", 0);
        PlayerPrefs.SetInt("Reward_ExtraProjectile", 0);
        PlayerPrefs.SetInt("TotalKills", 0);
    }

    // === Hotkeys ===
    void Update()
    {
        // Enter -> Start
        if (Input.GetKeyDown(KeyCode.Return))
            OnClickStart();

        // Esc -> close settings if open, otherwise quit
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            bool settingsOpen =
                settingsPanel &&
                ((settingsCG != null && settingsCG.alpha > 0.5f) || settingsPanel.activeSelf);

            if (settingsOpen) OnClickCloseSetting();
            else OnClickQuit();
        }
    }
}