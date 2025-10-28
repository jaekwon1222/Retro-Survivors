using UnityEngine;

public class InputHandler : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
            GameManager.Instance.StartNextWave(); // press N → next wave
        if (Input.GetKeyDown(KeyCode.K))
            GameManager.Instance.AddScore(10);    // press K → +10 score
    }
}