using UnityEngine;
using TMPro;

public class UpgradeStatusPanel : MonoBehaviour
{
    [Header("Targets")]
    public PlayerAimWeapon manualFire;      // finds on player if null
    public PlayerMovement movement;      // finds on player if null

    [Header("UI")]
    public TextMeshProUGUI txtTitle;
    public TextMeshProUGUI txtProjectiles;
    public TextMeshProUGUI txtPower;
    public TextMeshProUGUI txtFireRate;
    public TextMeshProUGUI txtMove;
    public TextMeshProUGUI txtRadius;
    public TextMeshProUGUI txtPierce;

    float _t;

    void Awake()
    {
        // lazy find player & components
        if (!manualFire || !movement)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p)
            {
                if (!manualFire) manualFire = p.GetComponent<PlayerAimWeapon>();
                if (!movement) movement = p.GetComponent<PlayerMovement>();
            }
        }
    }

    void Update()
    {
        // throttle to ~5 Hz
        _t += Time.unscaledDeltaTime;
        if (_t < 0.2f) return;
        _t = 0f;

        if (txtTitle && !txtTitle.isTextObjectScaleStatic)
            txtTitle.text = "UPGRADES";

        // read with null-safe defaults
        int projCount = manualFire ? manualFire.ProjectileCount : 0;
        int power = manualFire ? manualFire.ProjectileDamage : 0;

        // shots/sec from fire interval (guard divide by zero)
        float fireRate = manualFire && manualFire.FireInterval > 0f
                        ? 1f / manualFire.FireInterval : 0f;

        float moveSpd = movement ? movement.moveSpeed : 0f;

        // hit radius (optional field name; set 0 if missing)
        float radius = 0f;
        if (manualFire)
        {
            // try common field names
            // comment only: adapt if your field differs
            radius = manualFire.HitRadius; // if you use another name, expose it or add a getter
        }

        // write UI
        if (txtProjectiles) txtProjectiles.text = $"Projectiles: <b>{projCount}</b>";
        if (txtPower) txtPower.text = $"Power: <b>{power}</b>";
        if (txtFireRate) txtFireRate.text = $"Fire Rate: <b>{fireRate:0.0}/s</b>";
        if (txtMove) txtMove.text = $"Move Speed: <b>{moveSpd:0.00}</b>";
        if (txtRadius) txtRadius.text = $"Hit Radius: <b>{radius:0.00}</b>";
        if (txtPierce) txtPierce.text = $"Pierce: <b>{(manualFire ? manualFire.ProjectilePierce : 0)}</b>";
    }

    // optional: toggle with key
    void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.U))
            gameObject.SetActive(!gameObject.activeSelf);
    }
}