using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AchievementUI : MonoBehaviour
{
    [Header("Achievement Settings")]
    public string achievementKey = "Achievement_5Kills";   // override in Inspector
    public string rewardKey = "Reward_ExtraProjectile";
    public Sprite lockedSprite;
    public Sprite unlockedSprite;

    [Header("Special Condition Achievement")]
    public bool requiresTwoConditions = false;
    public string secondaryConditionKey = "";  // e.g. "Pressed_SaltyP"

    Button button;
    Image image;

    void Awake()
    {
        button = GetComponent<Button>();
        image = GetComponent<Image>();
        button.onClick.AddListener(OnClick);
    }

    void Start()
    {
        Refresh();
    }

    void Refresh()
    {
        bool unlocked = false;

        if (requiresTwoConditions)
        {
            // BOTH must be true
            bool cond1 = PlayerPrefs.GetInt(achievementKey, 0) == 1;
            bool cond2 = PlayerPrefs.GetInt(secondaryConditionKey, 0) == 1;

            unlocked = cond1 && cond2;
        }
        else
        {
            unlocked = PlayerPrefs.GetInt(achievementKey, 0) == 1;
        }

        bool rewarded = PlayerPrefs.GetInt(rewardKey, 0) == 1;

        if (unlocked)
        {
            image.sprite = unlockedSprite;
            button.interactable = !rewarded; // disable button if reward already claimed
        }
        else
        {
            image.sprite = lockedSprite;
            button.interactable = false;
        }
    }

    void OnClick()
    {
        if (requiresTwoConditions)
        {
            bool cond1 = PlayerPrefs.GetInt(achievementKey, 0) == 1;
            bool cond2 = PlayerPrefs.GetInt(secondaryConditionKey, 0) == 1;

            if (cond1 && cond2)
            {
                PlayerPrefs.SetInt(rewardKey, 1);
                Debug.Log("[Reward] Special achievement reward unlocked!");
                Refresh();
            }
            return;
        }

        if (PlayerPrefs.GetInt(achievementKey, 0) == 1)
        {
            PlayerPrefs.SetInt(rewardKey, 1);
            Debug.Log("[Reward] Achievement reward unlocked!");
            Refresh();
        }
    }
}
