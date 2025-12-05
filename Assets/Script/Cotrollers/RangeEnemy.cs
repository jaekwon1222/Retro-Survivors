using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class RangeEnemy : MonoBehaviour
{
    [Header("UI")]
    public Sprite hoverIcon;

    [Header("Stats")]
    public int maxHP = 3;
    public int hp = 3;
    public int contactDamage = 1;   // damage dealt to player on contact
    public int scoreValue = 10;
    public float moveSpeed = 2.5f;

    public Animator anim;

    [Header("Visual")]
    public Transform enemySprite;   // the sprite object you want to flip

    [Header("Ranged Attack")]
    public GameObject projectilePrefab;
    public float fireCooldown = 1.5f;
    public float projectileSpeed = 10f;
    public float fireRange = 6f;

    Vector2 knockbackVelocity = Vector2.zero;
    float knockbackTime = 0f;

    float lastFireTime = -999f;

    [NonSerialized] public Action<RangeEnemy> OnDead;

    Rigidbody2D rb;
    Transform player;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        hp = Mathf.Clamp(hp, 0, maxHP);
    }

    void Start()
    {
        var p = GameObject.FindGameObjectWithTag("Player");
        if (p) player = p.transform;
    }

    void Update()
    {

        if (knockbackTime > 0f)
        {
            rb.MovePosition(rb.position + knockbackVelocity * Time.deltaTime);
            knockbackTime -= Time.deltaTime;
            return; // skip normal movement while knockback
        }

        if (!player) return;

        // distance check
        float dist = Vector2.Distance(transform.position, player.position);
        FacePlayer();

        // --- MOVE ---
        if (dist > fireRange)
        {
            // move toward player
            Vector2 dir = (player.position - transform.position).normalized;
            rb.MovePosition(rb.position + dir * moveSpeed * Time.deltaTime);
        }
        else
        {
            // enemy stops & fires
            TryShoot();
        }

        if (dist > fireRange)
            anim.SetBool("IsMoving", true);
        else
            anim.SetBool("IsMoving", false);

    }

    void FacePlayer()
    {
        if (!player) return;

        Vector2 dir = player.position - transform.position;

        Vector3 scale = enemySprite.localScale;

        if (dir.x < 0)
            scale.x = -Mathf.Abs(scale.x);  // face left
        else
            scale.x = Mathf.Abs(scale.x);   // face right

        enemySprite.localScale = scale;
    }

    void TryShoot()
    {
        if (Time.time - lastFireTime < fireCooldown) return;
        lastFireTime = Time.time;

        if (!projectilePrefab) return;

        // find direction
        Vector2 dir = (player.position - transform.position).normalized;

        // spawn projectile
        Vector2 firePos = transform.position + (Vector3)(dir * 0.5f);
        GameObject obj = Instantiate(projectilePrefab, firePos, Quaternion.identity);


        // configure projectile
        Projectile p = obj.GetComponent<Projectile>();
        p.owner = transform;
        p.damage = contactDamage;            // reuse enemy damage
        p.speed = projectileSpeed;
        p.Fire(dir);

        Debug.Log("[Enemy] Fired projectile.");
    }

    public void TakeDamage(int dmg)
    {
        hp -= dmg;
        if (hp <= 0) Die();
    }

    public void TakeDamage(int dmg, Vector2 knockback)
    {
        hp -= dmg;
        if (knockback != Vector2.zero)
        {
            knockbackVelocity = knockback;
            knockbackTime = 0.1f; // knockback lasts 0.1 seconds
        }
        if (hp <= 0) Die();
    }

    void Die()
    {
        GameManager.Instance.AddScore(scoreValue);

        // --- Track total kills ---
        int kills = PlayerPrefs.GetInt("TotalKills", 0) + 1;
        PlayerPrefs.SetInt("TotalKills", kills);

        // Check for kill-based achievement unlock
        if (kills >= 5 && PlayerPrefs.GetInt("Achievement_5Kills", 0) == 0)
        {
            PlayerPrefs.SetInt("Achievement_5Kills", 1);
            Debug.Log("[Achievement] 5 Kills unlocked!");
        }

        OnDead?.Invoke(this);
        //SFXManager.Instance?.PlayEnemyDie(); // play enemy death sfx
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // only react to player
        if (!other.CompareTag("Player")) return;

        // try to find PlayerHealth on the player
        var playerHealth = other.GetComponent<PlayerHealth>();
        Vector3 hitPos = other.ClosestPoint(transform.position);

        if (playerHealth != null)
        {
            bool applied = playerHealth.TryHit(contactDamage, hitPos);
            if (applied)
            {
                Debug.Log($"[Enemy] Contact hit from {name}, contactDamage={contactDamage}");
            }
        }
        else
        {
            // fallback: directly damage player if PlayerHealth is missing
            Debug.LogWarning("[Enemy] PlayerHealth not found on Player, using direct damage");
            GameManager.Instance.DamagePlayer(contactDamage);
        }
    }

    //void OnMouseDown() => TakeDamage(1);
}