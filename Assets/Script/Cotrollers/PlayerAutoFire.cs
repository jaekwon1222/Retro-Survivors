using UnityEngine;

public class PlayerAutoFire : MonoBehaviour
{
    [Header("Projectile")]
    public Projectile projectilePrefab;   // bullet prefab
    public Transform muzzlePoint;         // spawn point; fallback = this

    [Header("Auto Fire")]
    public float fireInterval = 1.0f;     // seconds between shots
    public float detectRadius = 10f;      // enemy search radius
    public bool requireEnemyInRange = true; // fire only if enemy found
    public bool rotateProjectile = true;  // rotate to flight direction
    public float spawnOffset = 0.25f;     // push spawn forward to avoid overlap

    float timer;

    void Start()
    {
        if (!muzzlePoint) muzzlePoint = transform;
        timer = fireInterval; // first shot after interval
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer < fireInterval) return;

        Enemy target = FindNearestEnemy();
        if (target == null && requireEnemyInRange)
        {
            timer = 0f; // skip this cycle
            return;
        }

        Vector3 spawn = muzzlePoint.position;
        Vector2 dir = target
            ? (target.transform.position - spawn).normalized
            : (Vector2)transform.right;

        // spawn slightly in front to avoid instant collisions
        spawn += (Vector3)(dir * spawnOffset);

        Shoot(dir, spawn);
        timer = 0f;
    }

    Enemy FindNearestEnemy()
    {
        // simple scan; optimize later if needed
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        Enemy best = null;
        float bestSqr = Mathf.Infinity;
        Vector3 origin = muzzlePoint.position;

        foreach (var e in enemies)
        {
            if (!e || !e.isActiveAndEnabled || e.hp <= 0) continue;
            float sqr = (e.transform.position - origin).sqrMagnitude;
            if (sqr < bestSqr && sqr <= detectRadius * detectRadius)
            {
                bestSqr = sqr;
                best = e;
            }
        }
        return best;
    }

    void Shoot(Vector2 dir, Vector3 spawn)
    {
        if (!projectilePrefab) { Debug.LogWarning("[AutoFire] projectilePrefab missing"); return; }

        var proj = Instantiate(projectilePrefab, spawn, Quaternion.identity);
        if (rotateProjectile && dir.sqrMagnitude > 0.0001f)
        {
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            proj.transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
        proj.Fire(dir);
    }
}