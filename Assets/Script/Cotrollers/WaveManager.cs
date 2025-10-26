using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [Header("Refs")]
    public GameObject enemyPrefab;          // Enemy prefab to spawn
    public Camera targetCam;                // If null, will use Camera.main

    [Header("Counts per wave")]
    public int batchesPerWave = 1;
    public int initialPerBatch = 6;
    public int incrementPerWave = 3;
    public int maxWaves = 5;

    [Header("Edge spawn (camera-based)")]
    [Tooltip("Positive = spawn slightly INSIDE the screen edges; negative = outside")]
    public float edgeOffset = 0.3f;         // 0.2~0.5 feels close to the edge (inside)
    public float jitterRadius = 0.8f;       // small spread near the edge
    public float separation = 0.6f;         // min spacing to avoid stacking
    public int triesPerEnemy = 12;          // attempts to find a free spot

    [Header("Wave pacing")]
    public float nextWaveDelay = 0.8f;      // delay after last enemy dies
    public bool autoStartFirstWave = true;  // spawn first wave automatically on Start

    // runtime
    int currentWave = 1;
    int aliveEnemies = 0;

    void Awake()
    {
        if (!targetCam) targetCam = Camera.main;
    }

    void Start()
    {
        if (autoStartFirstWave) SpawnWave();
    }

    // -------------------------------------------------------------------------
    // Wave lifecycle
    // -------------------------------------------------------------------------
    public void SpawnWave()
    {
        if (!enemyPrefab)
        {
            Debug.LogError("[WaveManager] enemyPrefab is NULL", this);
            return;
        }

        // Update UI before spawn
        UIManager.Instance?.SetWave(currentWave);

        int perBatch = initialPerBatch + (currentWave - 1) * incrementPerWave;
        int spawned = 0;

        for (int b = 0; b < batchesPerWave; b++)
        {
            for (int i = 0; i < perBatch; i++)
            {
                Vector3 pos = FindFreeEdgeSpot();
                var go = Instantiate(enemyPrefab, pos, Quaternion.identity);

                // hook death to track wave completion
                var enemy = go.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.OnDead += OnEnemyDead;
                }

                aliveEnemies++;
                spawned++;
            }
        }

        Debug.Log($"[WaveManager] Wave {currentWave} spawned: {spawned} enemies (perBatch={perBatch})");
    }

    void OnEnemyDead(Enemy e)
    {
        // Prevent double counting
        if (e != null)
            e.OnDead -= OnEnemyDead;

        aliveEnemies = Mathf.Max(0, aliveEnemies - 1);
        Debug.Log($"[WaveManager] Enemy killed. {aliveEnemies} remaining in wave {currentWave}");

        // Only trigger next wave when all enemies are dead
        if (aliveEnemies > 0) return;

        Debug.Log($"[WaveManager] Wave {currentWave} cleared!");

        // Open upgrade menu once per cleared wave
        var upgr = FindAnyObjectByType<UpgradeManager>();
        if (upgr != null)
        {
            upgr.OpenForWave(currentWave, StartNextWave);
        }
        else
        {
            // Fallback: no UpgradeManager found, proceed to next wave after a delay
            Invoke(nameof(StartNextWave), nextWaveDelay);
        }
    }

    void StartNextWave()
    {
        currentWave++;
        UIManager.Instance?.SetWave(currentWave);  // reflect wave increase immediately
        SpawnWave();
    }

    // -------------------------------------------------------------------------
    // Spawn helpers
    // -------------------------------------------------------------------------
    Vector3 FindFreeEdgeSpot()
    {
        // try multiple times to avoid overlaps; fallback to last try
        Vector3 last = targetCam ? targetCam.transform.position : Vector3.zero;

        for (int i = 0; i < triesPerEnemy; i++)
        {
            Vector3 basePos = GetRandomEdgePositionInside(targetCam, edgeOffset);
            Vector2 off = Random.insideUnitCircle * jitterRadius;
            Vector3 tryPos = basePos + (Vector3)off;
            last = tryPos;

            if (!HasEnemyInRadius(tryPos, separation)) return tryPos;
        }
        return last;
    }

    // Spawn slightly INSIDE the screen edges when offset > 0
    static Vector3 GetRandomEdgePositionInside(Camera cam, float offset)
    {
        if (!cam) return Vector3.zero;

        Vector3 c = cam.transform.position;
        float halfH = cam.orthographicSize;
        float halfW = halfH * cam.aspect;

        // 0=left, 1=right, 2=top, 3=bottom
        int side = Random.Range(0, 4);
        float x, y;

        switch (side)
        {
            case 0: // left edge (inside)
                x = c.x - halfW + offset;
                y = c.y + Random.Range(-halfH, halfH);
                break;
            case 1: // right edge (inside)
                x = c.x + halfW - offset;
                y = c.y + Random.Range(-halfH, halfH);
                break;
            case 2: // top edge (inside)
                x = c.x + Random.Range(-halfW, halfW);
                y = c.y + halfH - offset;
                break;
            default: // bottom edge (inside)
                x = c.x + Random.Range(-halfW, halfW);
                y = c.y - halfH + offset;
                break;
        }
        return new Vector3(x, y, 0f);
    }

    // Tag-based overlap check (no layer setup required)
    bool HasEnemyInRadius(Vector3 pos, float radius)
    {
        var hits = Physics2D.OverlapCircleAll(pos, radius);
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i] && hits[i].CompareTag("Enemy")) return true;
        }
        return false;
    }

#if UNITY_EDITOR
    // small visual aid in Scene view
    void OnDrawGizmosSelected()
    {
        if (!targetCam) return;
        Gizmos.color = new Color(0f, 1f, 0f, 0.15f);

        var c = targetCam.transform.position;
        float halfH = targetCam.orthographicSize;
        float halfW = halfH * targetCam.aspect;

        // draw the inner spawn rectangle (edgeOffset inside)
        Vector3 bl = new Vector3(c.x - halfW + edgeOffset, c.y - halfH + edgeOffset, 0f);
        Vector3 tr = new Vector3(c.x + halfW - edgeOffset, c.y + halfH - edgeOffset, 0f);
        Vector3 br = new Vector3(tr.x, bl.y, 0f);
        Vector3 tl = new Vector3(bl.x, tr.y, 0f);

        Gizmos.DrawLine(bl, br); Gizmos.DrawLine(br, tr);
        Gizmos.DrawLine(tr, tl); Gizmos.DrawLine(tl, bl);
    }
#endif
}