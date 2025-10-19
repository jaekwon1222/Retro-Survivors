using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class EnemyMeleeAttack : MonoBehaviour
{
    public int damage = 1;          // damage per contact
    public float hitCooldown = 0.6f; // per-enemy cooldown (s)

    float lastHitTime = -999f;

    void OnCollisionEnter2D(Collision2D col)
    {
        TryHitPlayer(col);
    }

    void OnCollisionStay2D(Collision2D col)
    {
        // allow re-hit only after cooldown
        if (Time.time - lastHitTime >= hitCooldown)
            TryHitPlayer(col);
    }

    void TryHitPlayer(Collision2D col)
    {
        if (!col.collider.CompareTag("Player")) return;

        var hp = col.collider.GetComponent<PlayerHealth>();
        if (!hp) return;

        // use real physics contact point
        Vector3 hitPos = col.GetContact(0).point;

        // PlayerHealth has i-frames; this just debounces per-enemy
        if (hp.TryHit(damage, hitPos))
            lastHitTime = Time.time;
    }
}