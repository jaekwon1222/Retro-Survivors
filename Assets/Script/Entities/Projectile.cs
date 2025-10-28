using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Projectile : MonoBehaviour
{
    [Header("Flight")]
    public float speed = 12f;

    [Header("Damage")]
    public int damage = 1;

    [Tooltip("Splash damage radius on hit (0 = single target)")]
    public float aoeRadius = 0f;

    [Tooltip("How many extra enemies this projectile can pass through (0 = hits 1 enemy)")]
    public int pierce = 0;

    [SerializeField] LayerMask enemyMask; // optional: leave empty to hit everything with Enemy component

    Vector2 _dir = Vector2.right;
    Rigidbody2D _rb;

    HashSet<Enemy> _alreadyHit = new HashSet<Enemy>();
    int enemiesHit = 0;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        if (_rb)
        {
            _rb.gravityScale = 0f;
            _rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }

        // make sure collider triggers OnTriggerEnter2D
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    void Update()
    {
        // move by velocity if no rigidbody available
        if (_rb == null)
            transform.position += (Vector3)(_dir * speed * Time.deltaTime);
    }

    // --- API 1: minimal ---
    public void Fire(Vector2 direction)
    {
        _dir = direction.normalized;
        if (_rb) _rb.linearVelocity = _dir * speed;
        Debug.Log($"[Projectile] Fire() instance {GetInstanceID()} dir:{_dir} speed:{speed} pierce:{pierce} damage:{damage} aoe:{aoeRadius}");

    }

    // --- API 2: full (used when available) ---
    public void Fire(Vector2 direction, int dmg, float hitRadius)
    {
        damage = dmg;
        aoeRadius = Mathf.Max(0f, hitRadius);
        Fire(direction); // sets dir/velocity
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        var enemy = other.GetComponent<Enemy>();
        if (!enemy) return;
        if (_alreadyHit.Contains(enemy)) return; // prevent double damage on same enemy

        _alreadyHit.Add(enemy);
        enemiesHit++;

        Debug.Log($"[Projectile] Instance {GetInstanceID()} hit Enemy {enemy.name} (hp:{enemy.hp}). enemiesHit={enemiesHit} pierce={pierce}");


        enemy.TakeDamage(damage);

        // splash
        if (aoeRadius > 0f)
        {
            var hits = Physics2D.OverlapCircleAll(transform.position, aoeRadius,
                enemyMask.value == 0 ? Physics2D.DefaultRaycastLayers : enemyMask);

            foreach (var hit in hits)
            {
                var e = hit.GetComponent<Enemy>();
                if (e && !_alreadyHit.Contains(e))
                {
                    e.TakeDamage(damage);
                    _alreadyHit.Add(e);
                    Debug.Log($"[Projectile]  AO E damaged Enemy {e.name}");
                }
            }
        }

        // destroy only after exceeding pierce
        if (enemiesHit >= pierce)
        {
            Debug.Log($"[Projectile] Instance {GetInstanceID()} exceeded pierce ({pierce}) and will be destroyed.");
            Destroy(gameObject);
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (aoeRadius > 0f)
        {
            Gizmos.color = new Color(1f, 0.6f, 0f, 0.35f);
            Gizmos.DrawWireSphere(transform.position, aoeRadius);
        }
    }
#endif
}