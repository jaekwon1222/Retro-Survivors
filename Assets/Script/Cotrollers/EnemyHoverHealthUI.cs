using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyHoverHealthUI : MonoBehaviour
{
    public static EnemyHoverHealthUI Instance;

    public TMP_Text textElement;
    public Image iconImage; // <-- new Image field

    void Awake()
    {
        Instance = this;

        if (textElement == null)
            Debug.LogError("textElement NOT ASSIGNED!", this);

        if (iconImage == null)
            Debug.LogError("iconImage NOT ASSIGNED!", this);

        textElement.gameObject.SetActive(false);
        iconImage.gameObject.SetActive(false);
    }

    public void Show(MonoBehaviour enemy)
    {
        Sprite enemyIcon = null;
        int hp = 0;
        int maxHP = 0;

        if (enemy is Enemy e)
        {
            hp = e.hp;
            maxHP = e.maxHP;
            enemyIcon = e.hoverIcon;
        }
        else if (enemy is RangeEnemy r)
        {
            hp = r.hp;
            maxHP = r.maxHP;
            enemyIcon = r.hoverIcon;
        }

        textElement.text = $"{hp} / {maxHP}";
        textElement.gameObject.SetActive(true);

        if (enemyIcon != null)
        {
            iconImage.sprite = enemyIcon;
            iconImage.gameObject.SetActive(true);
        }
        else
        {
            iconImage.gameObject.SetActive(false);
        }
    }

    public void Hide()
    {
        textElement.gameObject.SetActive(false);
        iconImage.gameObject.SetActive(false);
    }
}
