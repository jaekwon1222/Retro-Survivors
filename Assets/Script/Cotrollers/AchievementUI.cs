using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AchievementUI : MonoBehaviour
{
    [Header("Achievement Settings")]
    public string achievementKey = "Achievement_5Kills";
    public string rewardKey = "Reward_ExtraProjectile";
    public Sprite lockedSprite;
    public Sprite unlockedSprite;

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
        bool unlocked = PlayerPrefs.GetInt(achievementKey, 0) == 1;
        bool rewarded = PlayerPrefs.GetInt(rewardKey, 0) == 1;

        if (unlocked)
        {
            image.sprite = unlockedSprite;
            button.interactable = !rewarded; // disable if already claimed
        }
        else
        {
            image.sprite = lockedSprite;
            button.interactable = false;
        }
    }

    void OnClick()
    {
        if (PlayerPrefs.GetInt(achievementKey, 0) == 1)
        {
            PlayerPrefs.SetInt(rewardKey, 1);
            Debug.Log("[Reward] Extra projectile unlocked!");
            Refresh();
        }
    }
}
