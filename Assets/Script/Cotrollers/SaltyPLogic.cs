using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
public class SaltyP : MonoBehaviour
{
    [Header("Settings")]
    public float moveSpeed = 5f;
    public int contactDamage = 1;
    public float attackRange = 0.5f;   // distance to deal damage
    public float hitCooldown = 0.5f;   // cooldown per enemy

    [Header("Visual")]
    public Animator anim;   // assign in Inspector
    private bool isAttacking = false;

    [Header("Visual")]
    public Transform spriteTransform; // assign the SaltyP visual (child) here

    Rigidbody2D rb;
    Transform target;
    float lastHitTime = -999f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void Update()
    {
        FindClosestEnemy();
        MoveTowardsTarget();
        HandleAnimations();
    }

    void HandleAnimations()
    {
        if (!target || !anim) return;

        float distance = Vector2.Distance(transform.position, target.position);

        if (distance <= attackRange)
        {
            if (!isAttacking)
            {
                isAttacking = true;
                anim.SetBool("IsWalking", false);
                anim.SetBool("IsAttacking", true);
            }
        }
        else
        {
            if (isAttacking)
            {
                isAttacking = false;
                anim.SetBool("IsAttacking", false);
            }
            anim.SetBool("IsWalking", true);
        }
    }

    void FindClosestEnemy()
    {
        List<Transform> potentialTargets = new List<Transform>();

        Enemy[] enemies = GameObject.FindObjectsOfType<Enemy>();
        foreach (var e in enemies) potentialTargets.Add(e.transform);

        RangeEnemy[] rangeEnemies = GameObject.FindObjectsOfType<RangeEnemy>();
        foreach (var re in rangeEnemies) potentialTargets.Add(re.transform);

        float minDist = Mathf.Infinity;
        target = null;

        foreach (var t in potentialTargets)
        {
            if (t == null) continue;
            float dist = Vector2.Distance(transform.position, t.position);
            if (dist < minDist)
            {
                minDist = dist;
                target = t;
            }
        }
    }


    void MoveTowardsTarget()
    {
        if (!target) return;

        Vector2 dir = (target.position - transform.position).normalized;
        rb.MovePosition(rb.position + dir * moveSpeed * Time.deltaTime);

        if (spriteTransform != null)
        {
            Vector3 scale = spriteTransform.localScale;
            scale.x = (dir.x < 0) ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
            spriteTransform.localScale = scale;
        }

        // check for damage
        if (Vector2.Distance(transform.position, target.position) <= attackRange)
        {
            TryDamageEnemy(target);
        }
    }

    void TryDamageEnemy(Transform enemyTransform)
    {
        if (Time.time - lastHitTime < hitCooldown) return;

        // Try normal Enemy
        Enemy enemy = enemyTransform.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(contactDamage);
            lastHitTime = Time.time;
            Debug.Log($"[SaltyP] Damaged enemy {enemy.name} for {contactDamage} HP");
            return;
        }

        // Try RangeEnemy
        RangeEnemy rangeEnemy = enemyTransform.GetComponent<RangeEnemy>();
        if (rangeEnemy != null)
        {
            rangeEnemy.TakeDamage(contactDamage);
            lastHitTime = Time.time;
            Debug.Log($"[SaltyP] Damaged RangeEnemy {rangeEnemy.name} for {contactDamage} HP");
            return;
        }
    }

}
