using UnityEngine;

public class MainMenuHotkeys : MonoBehaviour
{
    // assign in inspector
    public UIManager ui;

    void Update()
    {
        // press Enter/Return to start
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (ui != null) ui.OnClickStart();
        }
    }
}