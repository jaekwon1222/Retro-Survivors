using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    // Start → load play scene
    public void OnClickStart()
    {
        SceneManager.LoadScene("Scene_Entry");
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