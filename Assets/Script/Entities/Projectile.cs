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

    [SerializeField] LayerMask enemyMask; // optional: leave empty to hit everything with Enemy component

    Vector2 _dir = Vector2.right;
    Rigidbody2D _rb;

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
    }

    // --- API 2: full (used when available) ---
    public void Fire(Vector2 direction, int dmg, float hitRadius)
    {
        damage    = dmg;
        aoeRadius = Mathf.Max(0f, hitRadius);
        Fire(direction); // sets dir/velocity
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        var enemy = other.GetComponent<Enemy>();
        if (!enemy) return;

        // direct hit
        enemy.TakeDamage(damage);

        // splash
        if (aoeRadius > 0f)
        {
            var hits = Physics2D.OverlapCircleAll(transform.position, aoeRadius,
                enemyMask.value == 0 ? Physics2D.DefaultRaycastLayers : enemyMask);

            for (int i = 0; i < hits.Length; i++)
            {
                var e = hits[i].GetComponent<Enemy>();
                if (e) e.TakeDamage(damage);
            }
        }

        Destroy(gameObject);
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