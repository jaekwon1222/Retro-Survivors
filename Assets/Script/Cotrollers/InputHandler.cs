using UnityEngine;

public class InputHandler : MonoBehaviour
{
    // later: send input to Player controller
    void Update()
    {
        // read inputs; do nothing yet
        if (Input.GetKeyDown(KeyCode.Escape))
            GameManager.Instance.PauseToggle();
    }
}