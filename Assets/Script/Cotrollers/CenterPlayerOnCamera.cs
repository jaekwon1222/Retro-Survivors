using UnityEngine;

[RequireComponent(typeof(Transform))]
public class CenterPlayerOnCamera : MonoBehaviour
{
    public float zAtWorld = 0f; // player z (2D=0)

    void Start()
    {
        var cam = Camera.main;
        if (!cam) return;

        // get world center of camera view
        var p = cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, -cam.transform.position.z));
        p.z = zAtWorld;

        // if you use Rigidbody2D, set position via rb; else transform
        var rb = GetComponent<Rigidbody2D>();
        if (rb) rb.position = p;
        else transform.position = p;
    }
}