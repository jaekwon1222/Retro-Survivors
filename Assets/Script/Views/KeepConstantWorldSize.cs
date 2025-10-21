using UnityEngine;

public class KeepConstantWorldSize : MonoBehaviour
{
    [Header("World-space size you want to see")]
    public Vector3 worldScale = new Vector3(1.6f, 0.25f, 1f);

    void LateUpdate()
    {
        var ls = transform.lossyScale; // current world scale (includes parents)
        float sx = ls.x != 0 ? worldScale.x / ls.x : 1f;
        float sy = ls.y != 0 ? worldScale.y / ls.y : 1f;
        float sz = ls.z != 0 ? worldScale.z / ls.z : 1f;
        transform.localScale = new Vector3(sx, sy, sz);
    }
}