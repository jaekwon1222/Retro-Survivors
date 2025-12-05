using UnityEngine;

public class PlayerAimWeapon : MonoBehaviour
{
    public Transform weaponHolder;
    public Transform muzzlePoint;
    public Transform weaponSprite;
    public Transform playerSprite;
    public GameObject projectilePrefab;

    private MonoBehaviour lastHoveredEnemy;

    [Header("Stats (Upgradable)")]
    [SerializeField] private int projectileDamage = 1;
    [SerializeField] private int projectileCount = 1;
    [SerializeField] private float hitRadius = 0f;
    [SerializeField] private int projectilePierce = 1;
    [SerializeField] private float fireInterval = 0.2f;

    // --- PUBLIC GETTERS for UI ---
    public int ProjectileDamage => projectileDamage;
    public int ProjectileCount => projectileCount;
    public float HitRadius => hitRadius;
    public int ProjectilePierce => projectilePierce;
    public float FireInterval => fireInterval;

    private float fireTimer;
    public float projectileSpeed = 15f;

    // --- UPGRADE METHODS ---
    public void AddDamage(int delta)
    {
        projectileDamage += delta;
    }

    public void AddProjectiles(int delta)
    {
        projectileCount = Mathf.Max(1, projectileCount + delta);
    }

    public void AddHitRadius(float delta)
    {
        hitRadius = Mathf.Max(0f, hitRadius + delta);
    }

    public void AddPierce(int delta)
    {
        projectilePierce = Mathf.Max(0, projectilePierce + delta);
    }

    public void AddFireRateMultiplier(float multiplier)
    {
        fireInterval /= multiplier;
    }


    void Update()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        RotateWeaponHolder();
        HandleShooting();
        CheckEnemyHover();
    }

    void CheckEnemyHover()
    {
        if (EnemyHoverHealthUI.Instance == null)
            return;

        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mouseWorld.x, mouseWorld.y);

        RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);

        if (hit.collider)
        {
            MonoBehaviour hoveredEnemy = hit.collider.GetComponent<Enemy>() as MonoBehaviour;

            if (hoveredEnemy == null)
                hoveredEnemy = hit.collider.GetComponent<RangeEnemy>() as MonoBehaviour;

            if (hoveredEnemy != null)
            {
                if (hoveredEnemy != lastHoveredEnemy)
                    lastHoveredEnemy = hoveredEnemy;

                EnemyHoverHealthUI.Instance.Show(hoveredEnemy);
                return;
            }
        }

        lastHoveredEnemy = null;
        EnemyHoverHealthUI.Instance.Hide();
    }

    void RotateWeaponHolder()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        Vector2 dir = mousePos - transform.position;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        weaponHolder.rotation = Quaternion.Euler(0, 0, angle);

        Vector3 weaponScale = weaponSprite.localScale;
        // Check if we have assigned the playerBody, otherwise fallback to current transform (risky if children)
        Vector3 playerScale = playerSprite != null ? playerSprite.localScale : Vector3.one;

        if (dir.x < 0) // aiming left
        {
            // Flip Weapon (Y-axis because it's rotated)
            weaponScale.y = -Mathf.Abs(weaponScale.y);

            // Flip Player (X-axis to face left)
            if (playerSprite != null)
                playerScale.x = -Mathf.Abs(playerScale.x);
        }
        else // aiming right
        {
            // Normal Weapon
            weaponScale.y = Mathf.Abs(weaponScale.y);

            // Normal Player (X-axis to face right)
            if (playerSprite != null)
                playerScale.x = Mathf.Abs(playerScale.x);
        }

        weaponSprite.localScale = weaponScale;

        // Apply the calculated scale to the player body
        if (playerSprite != null)
            playerSprite.localScale = playerScale;
    }


    void HandleShooting()
    {
        fireTimer += Time.deltaTime;

        if (Input.GetMouseButton(0) && fireTimer >= fireInterval)
        {
            fireTimer = 0f;

            int count = projectileCount;
            float spread = 10f; // degrees

            for (int i = 0; i < count; i++)
            {
                float offset = (i - (count - 1) / 2f) * spread;

                GameObject obj = Instantiate(projectilePrefab, muzzlePoint.position,
                    muzzlePoint.rotation * Quaternion.Euler(0, 0, offset));

                Projectile proj = obj.GetComponent<Projectile>();

                // Apply stats like AutoFire does
                proj.damage = projectileDamage;
                proj.aoeRadius = hitRadius;
                proj.pierce = projectilePierce;

                Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
                if (rb)
                    rb.linearVelocity = obj.transform.right * projectileSpeed;

                obj.transform.right = muzzlePoint.right; // ensure correct direction
            }
        }
    }


}
