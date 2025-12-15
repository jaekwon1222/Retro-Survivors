using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Enemy : MonoBehaviour
{
    [Header("UI")]
    public Sprite hoverIcon;

    [Header("Stats")]
    public int maxHP = 3;   
    public int hp = 3;
    public int scoreValue = 10;
    public int contactDamage = 1;
    public float moveSpeed = 2.5f;

    public Animator anim;
    bool isAttacking = false;  
    public float attackRange = 0.8f;

    [Header("Visual")]
    public Transform enemySprite;

    Vector2 knockbackVelocity = Vector2.zero;
    float knockbackTime = 0f;


    [NonSerialized] public Action<Enemy> OnDead;

    Rigidbody2D rb;
    Transform player;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        Collider2D col = GetComponent<Collider2D>();
        col.isTrigger = true;

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
            anim.SetBool("IsMoving", false);
            return; // skip normal movement while knockback
        }

        if (!player) return;
        FacePlayer();
        HandleAttackAnimation();

        Vector2 dir = (player.position - transform.position).normalized;
        rb.MovePosition(rb.position + dir * moveSpeed * Time.deltaTime);

        anim.SetBool("IsMoving", true);
    }

    void HandleAttackAnimation()
    {
        if (!player) return;

        float dist = Vector2.Distance(transform.position, player.position);

        if (dist < attackRange && !isAttacking)
        {
            isAttacking = true;
            anim.SetBool("IsMoving", false);
            anim.SetBool("isAttacking", true);   // turn on animation
        }
        else if (dist >= attackRange && isAttacking)
        {
            isAttacking = false;
            anim.SetBool("isAttacking", false);  // turn off animation
        }
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

    void Die()
    {
        GameManager.Instance.AddScore(scoreValue);

        // --- Track total kills ---
        int kills = PlayerPrefs.GetInt("TotalKills", 0) + 1;
        PlayerPrefs.SetInt("TotalKills", kills);

        // --- Kill achievements ---
        if (kills >= 5 && PlayerPrefs.GetInt("Achievement_5Kills", 0) == 0)
        {
            PlayerPrefs.SetInt("Achievement_5Kills", 1);
            Debug.Log("[Achievement] 5 Kills unlocked!");
        }

        if (kills >= 25 && PlayerPrefs.GetInt("Achievement_25Kills", 0) == 0)
        {
            PlayerPrefs.SetInt("Achievement_25Kills", 1);
            Debug.Log("[Achievement] 25 Kills unlocked!");
        }

        if (kills >= 50 && PlayerPrefs.GetInt("Achievement_50Kills", 0) == 0)
        {
            PlayerPrefs.SetInt("Achievement_50Kills", 1);
            Debug.Log("[Achievement] 50 Kills unlocked!");
        }

        if (kills >= 100 && PlayerPrefs.GetInt("Achievement_100Kills", 0) == 0)
        {
            PlayerPrefs.SetInt("Achievement_100Kills", 1);
            Debug.Log("[Achievement] 100 Kills unlocked!");
        }

        if (kills >= 150 && PlayerPrefs.GetInt("Achievement_150Kills", 0) == 0)
        {
            PlayerPrefs.SetInt("Achievement_150Kills", 1);
            Debug.Log("[Achievement] 150 Kills unlocked!");
        }

        if (kills >= 200 && PlayerPrefs.GetInt("Achievement_200Kills", 0) == 0)
        {
            PlayerPrefs.SetInt("Achievement_200Kills", 1);
            Debug.Log("[Achievement] 200 Kills unlocked!");
        }

        OnDead?.Invoke(this);
        SFXManager.Instance?.PlayEnemyDie(); // play enemy death sfx
        Destroy(gameObject);
    }

void OnTriggerStay2D(Collider2D other)
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