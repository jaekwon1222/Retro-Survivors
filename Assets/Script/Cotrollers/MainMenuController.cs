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

    public void OnClickClearStats()
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

    void Update()
    {
        // Press Enter to Start
        if (Input.GetKeyDown(KeyCode.Return))
            OnClickStart();

        // Press Esc to Quit
        if (Input.GetKeyDown(KeyCode.Escape))
            OnClickQuit();
    }
}