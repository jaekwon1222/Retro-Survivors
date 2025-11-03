using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    // Start → load play scene
    public void OnClickStart()
    {
        SceneManager.LoadScene("Scene_Entry");
    }

    public void OnClickClearStats()
    {
        PlayerPrefs.SetInt("Achievement_5Kills", 0);
        PlayerPrefs.SetInt("Reward_ExtraProjectile", 0);
        PlayerPrefs.SetInt("TotalKills", 0);
    }

    public void OnClickAchievements()
    {
        SceneManager.LoadScene("Achievements_Scene");
    }

    public void OnClickBack()
    {
        SceneManager.LoadScene("Scene_MainMenu");
    }

    // Quit app (Editor stops Play)
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