using UnityEngine;
using TMPro;

public class UpgradeStatusPanelMelee : MonoBehaviour
{
    [Header("Targets")]
    public PlayerMovement movement;
    public PlayerMeleeWeapon melee;

    [Header("UI")]
    public TextMeshProUGUI txtTitle;
    public TextMeshProUGUI txtMove;

    [Header("Melee UI")]
    public TextMeshProUGUI txtMeleeDamage;
    public TextMeshProUGUI txtMeleeKnockback;
    public TextMeshProUGUI txtMeleeAttackRate;
    public TextMeshProUGUI txtMeleeBleed;

    float _t;

    void Awake()
    {
        var p = GameObject.FindGameObjectWithTag("Player");
        if (p)
        {
            if (!movement) movement = p.GetComponent<PlayerMovement>();
            if (!melee) melee = p.GetComponentInChildren<PlayerMeleeWeapon>();
        }
    }

    void Update()
    {
        // update 5× per second
        _t += Time.unscaledDeltaTime;
        if (_t < 0.2f) return;
        _t = 0f;

        if (txtTitle && !txtTitle.isTextObjectScaleStatic)
            txtTitle.text = "MELEE UPGRADES";

        float moveSpd = movement ? movement.moveSpeed : 0f;

        // MELEE STATS
        int meleeDamage = melee ? melee.damage : 0;
        float meleeKB = melee ? melee.knockbackMultiplier : 0f;
        float meleeRate = melee ? melee.attackRateMultiplier : 0f;
        bool meleeBleed = melee ? melee.enableBleed : false;

        // WRITE UI
        if (txtMove) txtMove.text = $"Move Speed: <b>{moveSpd:0.00}</b>";
        if (txtMeleeDamage) txtMeleeDamage.text = $"Damage: <b>{meleeDamage}</b>";
        if (txtMeleeKnockback) txtMeleeKnockback.text = $"Knockback: <b>{meleeKB:0.0}x</b>";
        if (txtMeleeAttackRate) txtMeleeAttackRate.text = $"Attack Rate: <b>{meleeRate:0.0}x</b>";
        if (txtMeleeBleed) txtMeleeBleed.text = $"Bleed: <b>{(meleeBleed ? "ON" : "OFF")}</b>";
    }

    void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.U))
            gameObject.SetActive(!gameObject.activeSelf);
    }
}
