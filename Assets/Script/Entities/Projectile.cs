using System.Collections.Generic;
using System.Linq;
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

    public Transform owner;


    [SerializeField] LayerMask enemyMask; // optional: leave empty to hit everything with Enemy component

    Vector2 _dir = Vector2.right;
    Rigidbody2D _rb;

    HashSet<Enemy> _alreadyHit = new HashSet<Enemy>();
    HashSet<RangeEnemy> _alreadyHitRange = new HashSet<RangeEnemy>();
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
        if (other.transform == owner) return;
        // --- PLAYER HIT ---
        var playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth)
        {
            Vector3 hitPos = other.ClosestPoint(transform.position);

            if (playerHealth.TryHit(damage, hitPos))
                Destroy(gameObject);

            return; // don't check enemy logic below
        }


        var enemy = other.GetComponent<Enemy>();
        if (enemy)
        {
            if (_alreadyHit.Contains(enemy)) return;

            _alreadyHit.Add(enemy);
            enemiesHit++;

            enemy.TakeDamage(damage);

            // SPLASH (melee)
            if (aoeRadius > 0f)
            {
                var hits = Physics2D.OverlapCircleAll(
                    transform.position,
                    aoeRadius,
                    enemyMask.value == 0 ? Physics2D.DefaultRaycastLayers : enemyMask
                );

                foreach (var hit in hits)
                {
                    var e = hit.GetComponent<Enemy>();
                    if (e && !_alreadyHit.Contains(e))
                    {
                        e.TakeDamage(damage);
                        _alreadyHit.Add(e);
                    }
                }
            }

            if (enemiesHit >= pierce)
                Destroy(gameObject);

            return; // IMPORTANT: stop here
        }

        var rangeEnemy = other.GetComponent<RangeEnemy>();
        if (rangeEnemy)
        {
            if (_alreadyHitRange.Contains(rangeEnemy)) return;

            _alreadyHitRange.Add(rangeEnemy);
            enemiesHit++;

            rangeEnemy.TakeDamage(damage);


            // SPLASH (range enemies)
            if (aoeRadius > 0f)
            {
                var hits = Physics2D.OverlapCircleAll(
                    transform.position,
                    aoeRadius,
                    enemyMask.value == 0 ? Physics2D.DefaultRaycastLayers : enemyMask
                );

                foreach (var hit in hits)
                {
                    var re = hit.GetComponent<RangeEnemy>();
                    if (re && !_alreadyHitRange.Contains(re))
                    {
                        re.TakeDamage(damage);
                        _alreadyHitRange.Add(re);
                    }
                }
            }

            if (enemiesHit >= pierce)
                Destroy(gameObject);

            return; // done
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