using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class PlayerMeleeWeapon : MonoBehaviour
{
    [Header("Refs")]
    public Transform weaponSprite;
    public Transform playerSprite;
    public Animator animator;
    public Transform weaponHolder;

    // Track last hovered enemy (any type)
    private MonoBehaviour lastHoveredEnemy;

    [Header("Melee Settings")]
    public int damage = 1;

    [Header("Melee Upgrade Stats")]
    public float knockbackMultiplier = 1f;
    public float attackRateMultiplier = 1f;
    public bool enableBleed = false;

    private BoxCollider2D boxCollider;
    private Vector2 dir;
    private bool isFacingLeft;

    void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        boxCollider.isTrigger = true; // ensure it's a trigger
    }

    void Update()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        RotateWeaponHolder();

        bool isPunching = Input.GetMouseButton(0);
        animator.SetBool("isPunching", isPunching && !isFacingLeft);
        animator.SetBool("isSwingingLeft", isPunching && isFacingLeft);

        CheckEnemyHover();
    }

    void CheckEnemyHover()
    {
        if (EnemyHoverHealthUI.Instance == null)
        {
            Debug.LogError("EnemyHoverHealthUI.Instance is NULL!");
            return;
        }

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

        // Not hovering any enemy
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
        Vector3 playerScale = playerSprite != null ? playerSprite.localScale : Vector3.one;

        dir = mousePos - transform.position;
        isFacingLeft = dir.x < 0;

        if (dir.x < 0) // aiming left
        {
            weaponScale.y = -Mathf.Abs(weaponScale.y);
            if (playerSprite != null) playerScale.x = -Mathf.Abs(playerScale.x);
        }
        else // aiming right
        {
            weaponScale.y = Mathf.Abs(weaponScale.y);
            if (playerSprite != null) playerScale.x = Mathf.Abs(playerScale.x);
        }

        weaponSprite.localScale = weaponScale;
        if (playerSprite != null) playerSprite.localScale = playerScale;
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (!Input.GetMouseButton(0)) return; // only damage if left mouse held

        Vector2 knockbackDir = (other.transform.position - playerSprite.position).normalized * knockbackMultiplier;

        var enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage, knockbackDir);
            return;
        }

        var rangeEnemy = other.GetComponent<RangeEnemy>();
        if (rangeEnemy != null)
        {
            rangeEnemy.TakeDamage(damage, knockbackDir);
        }
    }
}
