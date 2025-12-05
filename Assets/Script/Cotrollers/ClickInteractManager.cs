using UnityEngine;

public class ClickToggleObjects : MonoBehaviour
{
    [Header("Objects to toggle")]
    public GameObject objectToDisable;   // usually itself
    public GameObject objectToEnable;    // the other object

    [Header("Optional Achievement Key")]
    public string saltyPKey = "SaltyP_Pressed";        // set to "SaltyP_Pressed" if relevant
    public string companionRewardKey = "Reward_Companion";

    void Start()
    {
        // If companion reward is unlocked, turn off objectToDisable
        if (PlayerPrefs.GetInt(companionRewardKey, 0) == 1)
        {
            if (objectToDisable)
            {
                objectToDisable.SetActive(false);
                Debug.Log($"[Init] {objectToDisable.name} disabled because companion is unlocked.");
            }
        }
    }

    void OnMouseDown()
    {
        Debug.Log($"Clicked {gameObject.name}");

        if (PlayerPrefs.GetInt(saltyPKey, 0) == 0)
        {
            // Mark achievement as claimed
            PlayerPrefs.SetInt(saltyPKey, 1);
            PlayerPrefs.Save();

            Debug.Log($"[Achievement] {saltyPKey} unlocked!");

            // Optional: toggle objects
            if (objectToDisable) objectToDisable.SetActive(false);
            if (objectToEnable) objectToEnable.SetActive(true);
        }
        else
        {
            Debug.Log($"[Achievement] {saltyPKey} already claimed.");
        }
    }
}
