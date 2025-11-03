using UnityEngine;
using System.Reflection; // for safe reflection calls
using System.Collections.Generic;

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
    public WeaponController weaponController;


    [Header("Stats (Upgradable)")]
    [SerializeField] private int projectileDamage = 1;   // bullet damage
    [SerializeField] private int projectileCount = 1;    // bullets per shot
    [SerializeField] private float hitRadius = 0f;       // area hit radius on impact (0 = single target)
    [SerializeField] private int projectilePierce = 0;

    float timer;

    void Start()
    {
        if (!muzzlePoint) muzzlePoint = transform;
        timer = fireInterval; // first shot after interval

        projectilePierce = 1;
        Debug.Log($"[PlayerAutoFire] Start() projectilePierce={projectilePierce}");
    }

    void Update()
    {

        Enemy aimTarget = FindClosestEnemy(); // helper method below
        if (aimTarget && weaponController)
        {
            Vector2 dir = (aimTarget.transform.position - transform.position).normalized;
            weaponController.Aim(dir);
        }

        timer += Time.deltaTime;
        if (timer < fireInterval) return;

        Enemy[] targets = FindClosestEnemies(projectileCount);
        if (targets.Length == 0)
        {
            if (requireEnemyInRange)
            {
                timer = 0f; // skip this cycle if nothing to shoot
                return;
            }
            else
            {
                // fire forward even without target (optional behavior)
                Vector3 spawn = muzzlePoint.position;
                Vector2 dir = (Vector2)transform.right;
                spawn += (Vector3)(dir * spawnOffset);
                ShootSingle(dir, spawn);
                timer = 0f;
                return;
            }
        }

        // Fire one projectile per target
        foreach (var target in targets)
        {
            if (!target) continue;
            Vector3 spawn = muzzlePoint.position;
            Vector2 dir = (target.transform.position - spawn).normalized;
            spawn += (Vector3)(dir * spawnOffset);

            if (weaponController)
                weaponController.Aim(dir);

            ShootSingle(dir, spawn);
        }

        timer = 0f;
    }

    Enemy FindClosestEnemy()
    {
        var enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        Enemy closest = null;
        float minDist = Mathf.Infinity;
        Vector3 origin = muzzlePoint ? muzzlePoint.position : transform.position;

        foreach (var e in enemies)
        {
            if (!e || !e.isActiveAndEnabled || e.hp <= 0) continue;
            float dist = (e.transform.position - origin).sqrMagnitude;
            if (dist < minDist)
            {
                minDist = dist;
                closest = e;
            }
        }

        return closest;
    }

    // Find up to 'count' closest enemies within detectRadius
    Enemy[] FindClosestEnemies(int count)
    {
        var enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        var list = new List<Enemy>();
        Vector3 origin = muzzlePoint ? muzzlePoint.position : transform.position;
        float maxSqr = detectRadius * detectRadius;

        foreach (var e in enemies)
        {
            if (!e || !e.isActiveAndEnabled || e.hp <= 0) continue;
            float sqr = (e.transform.position - origin).sqrMagnitude;
            if (sqr <= maxSqr) list.Add(e);
        }

        list.Sort((a, b) =>
        {
            float da = (a.transform.position - origin).sqrMagnitude;
            float db = (b.transform.position - origin).sqrMagnitude;
            return da.CompareTo(db);
        });

        if (list.Count > count)
            list = list.GetRange(0, count);

        return list.ToArray();
    }

    void ShootSingle(Vector2 dir, Vector3 spawn)
    {
        if (!projectilePrefab)
        {
            Debug.LogWarning("[AutoFire] projectilePrefab missing");
            return;
        }

        var proj = Instantiate(projectilePrefab, spawn, Quaternion.identity);
        Debug.Log($"[PlayerAutoFire] Spawned projectile {proj.GetInstanceID()} at {spawn} towards {dir}");

        if (rotateProjectile && dir.sqrMagnitude > 0.0001f)
        {
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            proj.transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        // apply stats and call Fire using a safe, API-flexible path
        ApplyStatsAndFire(proj, dir);
    }

    // Try to call the most specific Projectile API available.
    // 1) Fire(Vector2, int, float)  -> damage + aoe supported
    // 2) Fire(Vector2) + set public fields "damage"/"aoeRadius" via reflection
    // 3) As fallback, just Fire(Vector2) if available
    void ApplyStatsAndFire(Projectile proj, Vector2 dir)
    {
        // Direct assignment (best, clear)
        proj.damage = projectileDamage;
        proj.aoeRadius = hitRadius;
        proj.pierce = projectilePierce;

        Debug.Log($"[PlayerAutoFire] Applied stats -> dmg:{projectileDamage} aoe:{hitRadius} pierce:{projectilePierce} to projectile instance {proj.GetInstanceID()}");

        // Call most specific Fire API if available
        var t = proj.GetType();
        var m3 = t.GetMethod("Fire", new[] { typeof(Vector2), typeof(int), typeof(float) });
        if (m3 != null)
        {
            m3.Invoke(proj, new object[] { dir, projectileDamage, hitRadius });
            return;
        }
        var m1 = t.GetMethod("Fire", new[] { typeof(Vector2) });
        if (m1 != null)
        {
            m1.Invoke(proj, new object[] { dir });
            return;
        }
    }

    // === Upgrade hooks ===
    public void AddDamage(int delta) { projectileDamage += delta; }
    public void AddProjectiles(int delta) { projectileCount = Mathf.Max(1, projectileCount + delta); }
    public void AddHitRadius(float delta) { hitRadius = Mathf.Max(0f, hitRadius + delta); }

    public void AddPierce(int delta)
    {
        projectilePierce = Mathf.Max(0, projectilePierce + delta);
    }

    public void AddFireRateMultiplier(float multiplier)
    {
        fireInterval /= multiplier; 
        Debug.Log($"[PlayerAutoFire] Fire interval changed -> {fireInterval:F3}s (faster by x{multiplier})");
    }
}