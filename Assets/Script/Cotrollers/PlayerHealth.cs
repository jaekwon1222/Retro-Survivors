using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Hit")]
    public float invincibleDuration = 0.6f;   // i-frames after a hit
    public GameObject hitEffectPrefab;        // blood effect prefab (optional)

    float lastHitTime = -999f;

    // returns true if hit applied
    public bool TryHit(int damage, Vector3 hitPos)
    {
        if (Time.time - lastHitTime < invincibleDuration) return false;

        lastHitTime = Time.time;

        // update global HP/UI
        GameManager.Instance.DamagePlayer(damage);

        // spawn effect at contact point
        if (hitEffectPrefab)
            Instantiate(hitEffectPrefab, hitPos, Quaternion.identity);

        return true;
    }
}