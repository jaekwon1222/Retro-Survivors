using System.Collections;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaveManager : MonoBehaviour
{
    public enum Difficulty { Easy, Medium, Hard, Endless }

    public Difficulty selectedDifficulty = Difficulty.Medium; // default

    public enum GameMode { Wave, Endless }
    public GameMode gameMode = GameMode.Wave;   // Set in Inspector

    [Header("Combat Selection")]
    public GameObject combatUI;
    public GameObject meleePlayer;
    public GameObject rangePlayer;
    private bool combatSelected = false;

    [Header("Upgrade Managers")]
    public GameObject meleeUpgradeManager;
    public GameObject rangeUpgradeManager;

    [Header("Upgrade HUDs")]
    public GameObject UpgradeHUDMelee;
    public GameObject UpgradeHUDRange;

    [Header("Cameras")]
    public GameObject meleeCamera;
    public GameObject rangeCamera;

    [Header("Level Complete")]
    public GameObject levelCompleteUI;
    public bool stopGameOnComplete = true;

    [Header("Endless Mode")]
    public int killsForUpgrade = 5;
    private int killsSinceUpgrade = 0;
    public float endlessSpawnInterval = 1.5f;
    private bool endlessActive = false;


    [Header("Refs")]
    public GameObject enemyPrefab;          // Enemy prefab to spawn
    public GameObject rangeEnemyPrefab;
    public Camera targetCam;                // If null, will use Camera.main

    [Header("Strong enemies")]
    public GameObject strongEnemyPrefab;     // strong enemy prefab
    public int strongStartCount = 2;         // strong enemies on wave 1
    public int strongIncrementPerWave = 1;   // +1 strong enemy per wave
    public float strongSpawnDelay = 1f;

    [Header("Spawn Points")]
    public Transform[] spawnPoints;
    public bool useSpawnPoints = false;

    [Header("Map-specific Spawn Groups")]
    public GameObject mallSpawnGroup;     // contains mall spawn points
    public GameObject theaterSpawnGroup;  // contains theater spawn points

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
        meleePlayer?.SetActive(false);
        rangePlayer?.SetActive(false);

        meleeCamera?.SetActive(false);
        rangeCamera?.SetActive(false);

        meleeUpgradeManager?.SetActive(false);
        rangeUpgradeManager?.SetActive(false);

        // open UI first
        if (combatUI)
            combatUI.SetActive(true);

        int diff = PlayerPrefs.GetInt("SelectedDifficulty", 1);
        selectedDifficulty = (Difficulty)diff;

        ApplyDifficulty();

        int selectedMap = PlayerPrefs.GetInt("SelectedMap", 0);

        if (selectedMap == 0)
        {
            // Mall map
            if (mallSpawnGroup) mallSpawnGroup.SetActive(true);
            if (theaterSpawnGroup) theaterSpawnGroup.SetActive(false);

            // Set spawnPoints list to mall points
            if (mallSpawnGroup)
                spawnPoints = mallSpawnGroup.GetComponentsInChildren<Transform>();
        }
        else
        {
            // Theater map
            if (mallSpawnGroup) mallSpawnGroup.SetActive(false);
            if (theaterSpawnGroup) theaterSpawnGroup.SetActive(true);

            // Set spawnPoints list to theater points
            if (theaterSpawnGroup)
                spawnPoints = theaterSpawnGroup.GetComponentsInChildren<Transform>();
        }

        Debug.Log("Selected Map = " + selectedMap);
        Debug.Log("SpawnPoints Assigned = " + spawnPoints.Length);


        //if (autoStartFirstWave) SpawnWave();
    }

    void Update()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ApplyDifficulty()
    {
        if (selectedDifficulty == Difficulty.Easy)
        {
            maxWaves = 5;
            initialPerBatch = 4;
            incrementPerWave = 2;

            strongStartCount = 2;
            strongIncrementPerWave = 1;
        }
        else if (selectedDifficulty == Difficulty.Medium)
        {
            maxWaves = 7;
            initialPerBatch = 6;
            incrementPerWave = 3;

            strongStartCount = 3;
            strongIncrementPerWave = 2;
        }
        else if (selectedDifficulty == Difficulty.Hard)
        {
            maxWaves = 10;
            initialPerBatch = 8;
            incrementPerWave = 4;

            strongStartCount = 4;
            strongIncrementPerWave = 3;
        }
        else if (selectedDifficulty == Difficulty.Endless)
        {
            maxWaves = 100;
            initialPerBatch = 12;
            incrementPerWave = 6;

            strongStartCount = 6;
            strongIncrementPerWave = 5;
        }

        Debug.Log($"[WaveManager] Difficulty set to: {selectedDifficulty}, Strong enemies start={strongStartCount}, increment={strongIncrementPerWave}");
    }

    void ApplyDifficultyToEnemy(Enemy enemy)
    {
        if (!enemy) return;

        switch (selectedDifficulty)
        {
            case Difficulty.Easy:
                enemy.maxHP = 3;
                enemy.hp = 3;
                enemy.moveSpeed = 2f;
                enemy.contactDamage = 1;
                break;

            case Difficulty.Medium:
                enemy.maxHP = 5;
                enemy.hp = 5;
                enemy.moveSpeed = 2.5f;
                enemy.contactDamage = 2;
                break;

            case Difficulty.Hard:
                enemy.maxHP = 8;
                enemy.hp = 8;
                enemy.moveSpeed = 3.5f;
                enemy.contactDamage = 3;
                break;

            case Difficulty.Endless:
                enemy.maxHP = 15;
                enemy.hp = 15;
                enemy.moveSpeed = 4.5f;
                enemy.contactDamage = 5;
                break;
        }

        // Optional: scale slightly per wave
        enemy.maxHP += (currentWave - 1) * 2;
        enemy.hp = enemy.maxHP;
        enemy.moveSpeed += (currentWave - 1) * 0.2f;
        enemy.contactDamage += (currentWave - 1) / 2;
    }



    public void SelectMelee()
    {
        PlayerPrefs.SetInt("SelectedCombat", 0);
        if (meleePlayer) meleePlayer.SetActive(true);
        if (rangePlayer) rangePlayer.SetActive(false);

        if (meleeCamera) meleeCamera.SetActive(true);
        if (rangeCamera) rangeCamera.SetActive(false);

        if (meleeUpgradeManager) meleeUpgradeManager.SetActive(true);
        if (rangeUpgradeManager) rangeUpgradeManager.SetActive(false);

        if (UpgradeHUDMelee) UpgradeHUDMelee.SetActive(true);
        if (UpgradeHUDRange) UpgradeHUDRange.SetActive(false);

        combatSelected = true;

        if (combatUI) combatUI.SetActive(false);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        var spawnMgr = FindObjectOfType<PlayerSpawnManager>();
        if (spawnMgr != null)
            spawnMgr.SpawnPlayer();

        if (gameMode == GameMode.Wave)
            SpawnWave();
        else
            StartEndlessMode();

    }

    public void SelectRange()
    {
        PlayerPrefs.SetInt("SelectedCombat", 1);
        if (meleePlayer) meleePlayer.SetActive(false);
        if (rangePlayer) rangePlayer.SetActive(true);

        if (meleeCamera) meleeCamera.SetActive(false);
        if (rangeCamera) rangeCamera.SetActive(true);

        if (meleeUpgradeManager) meleeUpgradeManager.SetActive(false);
        if (rangeUpgradeManager) rangeUpgradeManager.SetActive(true);

        if (UpgradeHUDMelee) UpgradeHUDMelee.SetActive(false);
        if (UpgradeHUDRange) UpgradeHUDRange.SetActive(true);

        combatSelected = true;

        if (combatUI) combatUI.SetActive(false);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        var spawnMgr = FindObjectOfType<PlayerSpawnManager>();
        if (spawnMgr != null)
            spawnMgr.SpawnPlayer();

        if (gameMode == GameMode.Wave)
            SpawnWave();
        else
            StartEndlessMode();

    }

    // ------------------------------------------------------------
    // Endless mode logic
    // ------------------------------------------------------------
    void StartEndlessMode()
    {
        endlessActive = true;
        StartCoroutine(EndlessSpawner());
    }

    IEnumerator EndlessSpawner()
    {
        while (endlessActive)
        {
            SpawnOneEnemy();
            yield return new WaitForSeconds(endlessSpawnInterval);
        }
    }

    void SpawnOneEnemy()
    {
        Vector3 pos = GetSpawnPoint();

        GameObject go;

        float rangeChance = 0.4f;

        if (rangeEnemyPrefab != null && Random.value < rangeChance)
        {
            go = Instantiate(rangeEnemyPrefab, pos, Quaternion.identity);
            var r = go.GetComponent<RangeEnemy>();
            if (r != null)
                r.OnDead += OnRangeEnemyDead;
        }
        else
        {
            go = Instantiate(enemyPrefab, pos, Quaternion.identity);
            var e = go.GetComponent<Enemy>();
            if (e != null)
            {
                ApplyDifficultyToEnemy(e);
                e.OnDead += OnEnemyDead;
            }
        }

        aliveEnemies++;
    }

    void OpenEndlessUpgrade()
{
    endlessActive = false; // pause spawner

    var upgr = FindAnyObjectByType<UpgradeManager>();
    if (upgr != null)
    {
        upgr.OpenForEndless(() =>
        {
            endlessActive = true;
            StartCoroutine(EndlessSpawner());
        });
    }
}



    // -------------------------------------------------------------------------
    // Wave lifecycle
    // -------------------------------------------------------------------------
    private Vector3 GetSpawnPoint()
    {
        // If we have explicit spawn points, pick a random one as base.
        if (useSpawnPoints && spawnPoints != null && spawnPoints.Length > 0)
        {
            // pick a base spawn point
            Vector3 basePos = spawnPoints[Random.Range(0, spawnPoints.Length)].position;

            Vector3 last = basePos;
            // try to find a free spot around the base point
            for (int i = 0; i < triesPerEnemy; i++)
            {
                Vector2 off = Random.insideUnitCircle * jitterRadius;
                Vector3 tryPos = basePos + (Vector3)off;
                last = tryPos;
                if (!HasEnemyInRadius(tryPos, separation)) return tryPos;
            }
            // fallback to the last tried position if none were free
            return last;
        }

        // fallback to original edge-based spawn logic
        return FindFreeEdgeSpot();
    }


    public void SpawnWave()
    {
        Debug.Log("WaveManager SpawnWave called from: " + gameObject.name);

        if (!combatSelected)
        {
            Debug.LogWarning("Tried to spawn wave before combat was selected!");
            return;
        }


        if (!enemyPrefab)
        {
            Debug.LogError("[WaveManager] enemyPrefab is NULL", this);
            return;
        }

        // Update UI before spawn
        UIManager.Instance?.SetWave(currentWave, maxWaves);

        int perBatch = initialPerBatch + (currentWave - 1) * incrementPerWave;
        int spawned = 0;

        for (int b = 0; b < batchesPerWave; b++)
        {
            for (int i = 0; i < perBatch; i++)
            {
                Vector3 pos = GetSpawnPoint();
                GameObject go;
                //var go = Instantiate(enemyPrefab, pos, Quaternion.identity);

                // hook death to track wave completion
                // Check for normal Enemy
                float chanceRangeEnemy = 0.5f; // 20% chance, adjust as needed
                if (rangeEnemyPrefab != null && Random.value < chanceRangeEnemy)
                {
                    go = Instantiate(rangeEnemyPrefab, pos, Quaternion.identity);
                    var rEnemy = go.GetComponent<RangeEnemy>();
                    if (rEnemy != null)
                    {
                        aliveEnemies++;
                        rEnemy.OnDead += OnRangeEnemyDead;
                    }
                }
                else
                {
                    go = Instantiate(enemyPrefab, pos, Quaternion.identity);
                    var enemy = go.GetComponent<Enemy>();
                    if (enemy != null)
                    {
                        ApplyDifficultyToEnemy(enemy);
                        enemy.OnDead += OnEnemyDead;
                        aliveEnemies++;
                    }
                }
                spawned++;
            }
        }

        // schedule strong enemies spawn after a short delay
        if (strongEnemyPrefab && strongSpawnDelay >= 0f)
        {
            StartCoroutine(SpawnStrongAfterDelay());
        }

        Debug.Log($"[WaveManager] Wave {currentWave} spawned: {spawned} enemies (perBatch={perBatch})");

        StartCoroutine(SpawnStrongAfterDelay());

        Debug.Log($"[WaveManager] Wave {currentWave} spawned: {spawned} normal enemies");
    }

    void OnEnemyDead(Enemy e)
    {
        // Prevent double counting
        if (e != null)
            e.OnDead -= OnEnemyDead;

        aliveEnemies = Mathf.Max(0, aliveEnemies - 1);
        Debug.Log($"[WaveManager] Enemy killed. {aliveEnemies} remaining in wave {currentWave}");

        // ------------------------------------------
        // ENDLESS MODE handling
        // ------------------------------------------
        if (gameMode == GameMode.Endless)
        {
            killsSinceUpgrade++;

            if (killsSinceUpgrade >= killsForUpgrade)
            {
                killsSinceUpgrade = 0;
                OpenEndlessUpgrade();
            }

            return;
        }

        // Only trigger next wave when all enemies are dead
        if (aliveEnemies > 0) return;

        Debug.Log($"[WaveManager] Wave {currentWave} cleared!");

        if (currentWave >= maxWaves)
        {
            Debug.Log("[WaveManager] All waves completed!");

            if (levelCompleteUI)
                levelCompleteUI.SetActive(true);

            if (stopGameOnComplete)
                Time.timeScale = 0f;   // freeze the game
            return;   
        }

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

    void OnRangeEnemyDead(RangeEnemy r)
    {
        // Unsubscribe so event doesn't fire twice
        if (r != null)
            r.OnDead -= OnRangeEnemyDead;

        aliveEnemies = Mathf.Max(0, aliveEnemies - 1);
        Debug.Log($"[WaveManager] Range enemy killed. {aliveEnemies} remaining in wave {currentWave}");

        // Endless mode logic
        if (gameMode == GameMode.Endless)
        {
            killsSinceUpgrade++;

            if (killsSinceUpgrade >= killsForUpgrade)
            {
                killsSinceUpgrade = 0;
                OpenEndlessUpgrade();
            }
            return;
        }

        // Normal wave mode
        if (aliveEnemies > 0) return;

        WaveClearedLogic();
    }

    void WaveClearedLogic()
    {
        Debug.Log($"[WaveManager] Wave {currentWave} cleared!");

        if (currentWave >= maxWaves)
        {
            Debug.Log("[WaveManager] All waves completed!");

            if (levelCompleteUI)
                levelCompleteUI.SetActive(true);

            if (stopGameOnComplete)
                Time.timeScale = 0f;
            return;
        }

        // Show upgrade screen
        var upgr = FindAnyObjectByType<UpgradeManager>();
        if (upgr != null)
        {
            upgr.OpenForWave(currentWave, StartNextWave);
        }
        else
        {
            Invoke(nameof(StartNextWave), nextWaveDelay);
        }
    }

    void StartNextWave()
    {
        currentWave++;
        UIManager.Instance?.SetWave(currentWave, maxWaves);  // reflect wave increase immediately
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

    // -------------------------------------------------------------------------
    // Strong enemies spawn
    // -------------------------------------------------------------------------
    IEnumerator SpawnStrongAfterDelay()
    {
        if (!strongEnemyPrefab) yield break;

        // optional early-out if config is effectively disabled
        if (strongStartCount <= 0 && strongIncrementPerWave <= 0)
            yield break;

        if (strongSpawnDelay > 0f)
            yield return new WaitForSeconds(strongSpawnDelay);

        int strongCount = strongStartCount + (currentWave - 1) * strongIncrementPerWave;
        if (strongCount <= 0) yield break;

        int spawned = 0;
        for (int i = 0; i < strongCount; i++)
        {
            Vector3 pos = GetSpawnPoint();
            var go = Instantiate(strongEnemyPrefab, pos, Quaternion.identity);

            // Check for normal Enemy
            var enemy = go.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.OnDead += OnEnemyDead;

                ApplyDifficultyToEnemy(enemy);
            }
            else
            {
                // Check for RangeEnemy
                var rEnemy = go.GetComponent<RangeEnemy>();
                if (rEnemy != null)
                {
                    rEnemy.OnDead += OnRangeEnemyDead;
                }
            }

            aliveEnemies++;
            spawned++;
        }

        Debug.Log($"[WaveManager] Wave {currentWave} spawned {spawned} strong enemies");
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