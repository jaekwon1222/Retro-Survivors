using System.Linq;
using UnityEngine;

[DisallowMultipleComponent]
public class EnemyHealthBar : MonoBehaviour
{
    [Header("Prefab (required)")]
    public GameObject hpBarPrefab;      // assign HPBar prefab (root with BG/Fill under it)

    [Header("Layout")]
    public float yOffset = 1.3f;        // height above enemy head
    public float width   = 1.6f;        // full bar width
    public float height  = 0.25f;       // bar thickness

    [Header("Auto-wire names (case-insensitive)")]
    public string fillName = "Fill";
    public string bgName   = "BG";

    private Enemy enemy;
    private Transform bar;              // spawned HPBar root
    private SpriteRenderer fill;        // child sprite (green)
    private SpriteRenderer bg;          // child sprite (background)

    void Awake()
    {
        enemy = GetComponent<Enemy>();
        if (!enemy)
        {
            Debug.LogError("[EnemyHealthBar] Enemy component missing.", this);
            enabled = false;
        }
    }

    void OnEnable()
    {
        if (!enabled) return;

        if (!hpBarPrefab)
        {
            Debug.LogError("[EnemyHealthBar] hpBarPrefab is not assigned.", this);
            enabled = false;
            return;
        }

        CleanupBar();

        // spawn (detached so parent scale does not affect size)
        bar = Instantiate(hpBarPrefab, transform.position, Quaternion.identity).transform;
        bar.name = $"HPBar_{gameObject.name}";

        // robust auto-wire: try name match first (case-insensitive), then fallback by sorting order
        AutoWireChildren(bar);

        // enforce rendering defaults (avoid pink/lighting/order issues)
        ForceSpriteDefaults(bg,   order: 3);
        ForceSpriteDefaults(fill, order: 4);

        enemy.OnDead += HandleEnemyDead;
    }

    void LateUpdate()
    {
        if (!bar || !enemy) return;

        // follow (pixel snap for stability; PPU=100)
        Vector3 p = enemy.transform.position + new Vector3(0f, yOffset, 0f);
        const float PPU = 100f;
        p.x = Mathf.Round(p.x * PPU) / PPU;
        p.y = Mathf.Round(p.y * PPU) / PPU;
        bar.position = p;

        // ratio
        float ratio = Mathf.Clamp01((float)enemy.hp / Mathf.Max(1, enemy.maxHP));
        float w = width * ratio;

        // update fill/bg geometry
        if (fill)
        {
            fill.transform.localScale    = new Vector3(w, height, 1f);
            fill.transform.localPosition = new Vector3(-width * 0.5f + w * 0.5f, 0f, -0.001f);
        }
        if (bg)
        {
            bg.transform.localScale    = new Vector3(width, height + 0.02f, 1f);
            bg.transform.localPosition = Vector3.zero;
        }
    }

    void HandleEnemyDead(Enemy e) => CleanupBar();

    void OnDisable()
    {
        if (enemy != null) enemy.OnDead -= HandleEnemyDead;
        CleanupBar();
    }

    void OnDestroy()
    {
        if (enemy != null) enemy.OnDead -= HandleEnemyDead;
        CleanupBar();
    }

    // -------- helpers --------

    void AutoWireChildren(Transform root)
    {
        // 1) name-based (case-insensitive)
        var all = root.GetComponentsInChildren<Transform>(true);
        var fillT = all.FirstOrDefault(t => t.name.ToLower().Contains(fillName.ToLower()));
        var bgT   = all.FirstOrDefault(t => t.name.ToLower().Contains(bgName.ToLower()));

        // 2) If names not found, fallback: pick two SpriteRenderers and
        //    assume the one with higher order is Fill, lower is BG.
        if (!fillT || !bgT)
        {
            var srs = root.GetComponentsInChildren<SpriteRenderer>(true);
            if (srs.Length >= 2)
            {
                var ordered = srs.OrderBy(sr => sr.sortingOrder).ToArray();
                bg   = bg   ? bg   : ordered[0];
                fill = fill ? fill : ordered[^1];
            }
        }

        // If we found by name, cache SRs now.
        if (!fill && fillT) fill = fillT.GetComponent<SpriteRenderer>();
        if (!bg   && bgT)   bg   = bgT.GetComponent<SpriteRenderer>();

        if (!fill || !bg)
        {
            Debug.LogWarning("[EnemyHealthBar] Could not auto-wire BG/Fill. " +
                             "Make sure prefab children are named like 'BG' and 'Fill' or adjust fillName/bgName.", root);
        }
    }

    void ForceSpriteDefaults(SpriteRenderer sr, int order)
    {
        if (!sr) return;
        sr.sortingLayerName = "Characters";
        sr.sortingOrder     = order;

        // Ensure unlit sprite material (avoid pink/lighting in URP).
        // If your project uses URP, set an Unlit Sprite material on the prefab.
        // As a safe fallback, use built-in sprite mat:
        if (!sr.sharedMaterial || sr.sharedMaterial.name.ToLower().Contains("lit"))
        {
            var mat = Resources.GetBuiltinResource<Material>("Sprites-Default.mat");
            if (mat) sr.material = mat;
        }
    }

    void CleanupBar()
    {
        if (bar) Destroy(bar.gameObject);
        bar = null; fill = null; bg = null;
    }
}