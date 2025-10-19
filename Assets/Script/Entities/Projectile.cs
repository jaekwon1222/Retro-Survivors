using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Projectile : MonoBehaviour
{
    public int damage = 1;
    public float speed = 14f;
    public float lifeTime = 3f;
    [Tooltip("Only collide with these layers (set to Enemy)")]
    public LayerMask hitLayers = ~0;

    Rigidbody2D rb;
    bool consumed; // ensure single hit

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    // set velocity and self-destroy
    public void Fire(Vector2 dir)
    {
        rb.linearVelocity = dir.normalized * speed;
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (consumed) return;

        // layer filter
        if (((1 << other.gameObject.layer) & hitLayers) == 0) return;

        // find enemy component
        var enemy = other.GetComponent<Enemy>() ?? other.GetComponentInParent<Enemy>();
        if (enemy == null) return;

        consumed = true;
        enemy.TakeDamage(damage);
        Destroy(gameObject);
    }
}