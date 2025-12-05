using UnityEngine;

public class PlayerSpawnManager : MonoBehaviour
{
    [Header("Spawn Points Per Map")]
    public Transform mallSpawnPoint;        // map 0
    public Transform theaterSpawnPoint;     // map 1

    [Header("Player Objects")]
    public GameObject meleePlayer;
    public GameObject rangePlayer;

    [Header("Extra Object to Spawn With Player")]
    public GameObject companionPrefab;  

    void Start()
    {
        
    }

    public void SpawnPlayer()
    {
        int selectedMap = PlayerPrefs.GetInt("SelectedMap", -1);
        int selectedCombat = PlayerPrefs.GetInt("SelectedCombat", -1);

        if (selectedMap == -1)
        {
            Debug.LogWarning("No map selected — cannot spawn player!");
            return;
        }

        if (selectedCombat == -1)
        {
            Debug.LogWarning("No combat type selected — cannot spawn player!");
            return;
        }

        Transform spawn = (selectedMap == 0) ? mallSpawnPoint : theaterSpawnPoint;
        Debug.Log("Map Spawn = " + (selectedMap == 0 ? "Mall" : "Theater"));

        GameObject player = (selectedCombat == 0) ? meleePlayer : rangePlayer;
        GameObject other = (selectedCombat == 0) ? rangePlayer : meleePlayer;

        player.SetActive(true);
        other.SetActive(false);

        player.transform.SetPositionAndRotation(spawn.position, spawn.rotation);
        Debug.Log("Spawned " + (selectedCombat == 0 ? "MELEE" : "RANGE") + " player");

        SpawnExtraPrefab(spawn);
    }


    private void SpawnExtraPrefab(Transform spawn)
    {
        if (companionPrefab == null)
        {
            Debug.Log("No companion prefab assigned — skipping.");
            return;
        }

        GameObject obj = Instantiate(companionPrefab, spawn.position, spawn.rotation);
        Debug.Log($"Spawned companion prefab: {obj.name}");
    }
}
