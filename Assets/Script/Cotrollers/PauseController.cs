using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseController : MonoBehaviour
{
    [Header("Refs")]
    public GameObject pauseUI; // root: PauseUI

    public static bool IsPaused { get; private set; }

    void Start()
    {
        SetPaused(false); // ensure unpaused on scene start
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePause();
    }

    public void TogglePause()
    {
        SetPaused(!IsPaused);
    }

    public void SetPaused(bool pause)
    {
        IsPaused = pause;

        // show/hide popup
        if (pauseUI) pauseUI.SetActive(pause);

        // pause time-based systems
        Time.timeScale = pause ? 0f : 1f;

        // optional: pause audio & show cursor
        AudioListener.pause = pause;
        Cursor.visible = pause;
        Cursor.lockState = pause ? CursorLockMode.None : CursorLockMode.Locked;
    }

    // UI Button: Resume
    public void OnClickResume()
    {
        SetPaused(false);
    }

    // UI Button: Exit to main menu scene
    public void OnClickExit()
    {
        // restore state before scene change
        Time.timeScale = 1f;
        AudioListener.pause = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        SceneManager.LoadScene("Scene_MainMenu");
    }
}