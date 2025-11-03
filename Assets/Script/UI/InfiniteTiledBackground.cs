using UnityEngine;

public class InfiniteTiledBackground : MonoBehaviour
{
    public Sprite sprite;                 // Sprite used for tiling background
    public Transform cam;                 // Main camera (auto-assign if left empty)
    public string sortingLayerName = "Background";
    public int orderInLayer = -10;        // Keeps it behind other elements

    private Vector2 tileSize;
    private Transform[,] tiles = new Transform[3, 3];

    void Awake()
    {
        if (!cam) cam = Camera.main ? Camera.main.transform : null;
        if (!cam || !sprite)
        {
            Debug.LogError("[InfiniteTiledBackground] Camera or sprite not assigned.");
            enabled = false;
            return;
        }

        // Calculate the sprite’s world size
        var probe = new GameObject("size_probe");
        var sr = probe.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        tileSize = sr.bounds.size;        // World-space width and height
        Destroy(probe);

        // Create a 3x3 grid of tiles centered on the origin
        for (int y = 0; y < 3; y++)
        for (int x = 0; x < 3; x++)
        {
            var go = new GameObject($"tile_{x-1}_{y-1}");
            go.transform.SetParent(transform, false);
            go.transform.localPosition = new Vector3((x - 1) * tileSize.x, (y - 1) * tileSize.y, 0f);

            var r = go.AddComponent<SpriteRenderer>();
            r.sprite = sprite;
            r.sortingLayerName = sortingLayerName;
            r.sortingOrder = orderInLayer;

            tiles[x, y] = go.transform;
        }

        // Align tiles to camera position initially
        RecenterToCamera();
    }

    void LateUpdate()
    {
        RecenterToCamera();
    }

    private void RecenterToCamera()
    {
        var p = cam.position;
        // Snap grid position to nearest tile unit
        float baseX = Mathf.Floor(p.x / tileSize.x) * tileSize.x;
        float baseY = Mathf.Floor(p.y / tileSize.y) * tileSize.y;
        transform.position = new Vector3(baseX, baseY, transform.position.z);
    }
}