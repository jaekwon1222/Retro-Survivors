using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Panels")]
    public GameObject settingsPanel;     // Settings panel root (can be a simple GO or a panel with CanvasGroup)

    // Cached CanvasGroup (if the panel uses it)
    CanvasGroup settingsCG;

    [Header("Debug")]
    [SerializeField] bool openSettingsOnStart = false;

    void Awake()
    {
        // Ensure we have a reference even if not assigned in Inspector
        if (!settingsPanel)
        {
            var found = GameObject.Find("SettingsPanel");
            if (found) settingsPanel = found;
        }
        if (settingsPanel) settingsCG = settingsPanel.GetComponent<CanvasGroup>();
    }

    void Start()
    {
        if (!settingsPanel)
        {
            Debug.LogWarning("[MainMenu] settingsPanel is NOT assigned and could not be found by name.");
            return;
        }
        Debug.Log($"[MainMenu] SettingsPanel ref ok. activeSelf={settingsPanel.activeSelf}, hasCG={(settingsCG!=null)}");
        if (openSettingsOnStart) ShowPanel(settingsPanel, true);
    }

    // === Scene Navigation ===
    public void OnClickStart()
    {
        SceneManager.LoadScene("Scene_Entry");
    }

    public void OnClickAchievements()
    {
        SceneManager.LoadScene("Achievements_Scene");
    }

    public void OnClickBack()
    {
        SceneManager.LoadScene("Scene_MainMenu");
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