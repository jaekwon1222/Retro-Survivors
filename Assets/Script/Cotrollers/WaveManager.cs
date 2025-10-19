using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [Header("Prefabs")]
    public Enemy enemyPrefab;

    [Header("Wave Config")]
    public int maxWaves = 5;                 // total waves
    public int batchesPerWave = 6;           // batches per wave
    public int enemiesPerBatchInitial = 5;   // start: 5 per batch
    public int enemiesPerBatchIncrement = 2; // +2 per wave
    public float interBatchDelay = 0.5f;     // small delay between batches

    [Header("Spawn (Camera-based)")]
    [Range(0f, 0.45f)]
    public float viewportMargin = 0.08f;     // 0=screen edge, 0.08=8% inside
    public bool useEightPoints = false;      // 4 corners or 8 points (corners+mids)

    // runtime
    readonly List<Vector3> _spawnPositions = new();
    readonly List<Enemy> _alive = new();

    int _currentWave = 1;
    int _currentBatchIndex = 0;
    int _enemiesPerBatch = 0;

    void Start()
    {
        if (!enemyPrefab)
        {
            Debug.LogError("[WaveManager] Enemy Prefab not assigned.");
            enabled = false;
            return;
        }

        _currentWave = GameManager.Instance.currentWave; // usually 1
        UIManager.Instance.SetWave(_currentWave);
        UIManager.Instance.SetScore(GameManager.Instance.score);

        RebuildSpawnPositions();
        StartCoroutine(RunWaves());
    }

    // ─────────────────────────────────────────────────────────────────────────────

    void RebuildSpawnPositions()
    {
        _spawnPositions.Clear();

        var cam = Camera.main;
        if (!cam)
        {
            Debug.LogError("[WaveManager] No Main Camera found.");
            return;
        }

        float m = viewportMargin;
        // 4 corners (inside the view by margin)
        Vector3 TL = ViewToWorld(cam, m, 1f - m);
        Vector3 TR = ViewToWorld(cam, 1f - m, 1f - m);
        Vector3 BL = ViewToWorld(cam, m, m);
        Vector3 BR = ViewToWorld(cam, 1f - m, m);

        _spawnPositions.Add(TL);
        _spawnPositions.Add(TR);
        _spawnPositions.Add(BL);
        _spawnPositions.Add(BR);

        if (useEightPoints)
        {
            // mids on each edge
            Vector3 TM = ViewToWorld(cam, 0.5f, 1f - m);
            Vector3 BM = ViewToWorld(cam, 0.5f, m);
            Vector3 LM = ViewToWorld(cam, m, 0.5f);
            Vector3 RM = ViewToWorld(cam, 1f - m, 0.5f);

            _spawnPositions.Add(TM);
            _spawnPositions.Add(BM);
            _spawnPositions.Add(LM);
            _spawnPositions.Add(RM);
        }
    }

    static Vector3 ViewToWorld(Camera cam, float vx, float vy)
    {
        var p = cam.ViewportToWorldPoint(new Vector3(vx, vy, -cam.transform.position.z));
        p.z = 0f;
        return p;
    }

    IEnumerator RunWaves()
    {
        while (_currentWave <= maxWaves)
        {
            _enemiesPerBatch = enemiesPerBatchInitial + (_currentWave - 1) * enemiesPerBatchIncrement;
            _currentBatchIndex = 0;
            Debug.Log($"[WaveManager] Wave {_currentWave} start | perBatch={_enemiesPerBatch}");

            for (int b = 0; b < batchesPerWave; b++)
            {
                _currentBatchIndex = b + 1;
                SpawnBatch(_enemiesPerBatch);
                // wait until current batch is cleared
                yield return new WaitUntil(() => _alive.Count == 0);
                yield return new WaitForSeconds(interBatchDelay);
            }

            if (_currentWave < maxWaves)
            {
                GameManager.Instance.StartNextWave();
                _currentWave = GameManager.Instance.currentWave;
                RebuildSpawnPositions(); // in case resolution/camera changed
            }
            else
            {
                Debug.Log("[WaveManager] All waves completed!");
                break;
            }
        }
    }

    void SpawnBatch(int count)
    {
        _alive.RemoveAll(e => e == null);

        for (int i = 0; i < count; i++)
        {
            var pos = _spawnPositions[i % _spawnPositions.Count];
            var e = Instantiate(enemyPrefab, pos, Quaternion.identity);
            e.OnDead += HandleEnemyDead;
            _alive.Add(e);
        }
        Debug.Log($"[WaveManager] Spawned batch {_currentBatchIndex}/{batchesPerWave} (x{count})");
    }

    void HandleEnemyDead(Enemy e)
    {
        if (e != null) e.OnDead -= HandleEnemyDead;
        _alive.Remove(e);
    }

    // gizmos to visualize spawn positions in Scene view
    void OnDrawGizmosSelected()
    {
        RebuildSpawnPositions();
        Gizmos.color = Color.red;
        foreach (var p in _spawnPositions)
            Gizmos.DrawWireSphere(p, 0.4f);
    }
}