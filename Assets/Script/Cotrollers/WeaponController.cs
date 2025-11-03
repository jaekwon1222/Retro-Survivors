using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Tooltip("Optional rotation smoothing")]
    public float rotationSpeed = 15f;

    Vector2 _targetDirection = Vector2.right;
    public void Aim(Vector2 direction)
    {
        if (direction.sqrMagnitude < 0.001f)
            return;

        _targetDirection = direction.normalized;
        float angle = Mathf.Atan2(_targetDirection.y, _targetDirection.x) * Mathf.Rad2Deg;

        // Smooth rotation
        transform.rotation = Quaternion.Lerp(transform.rotation,
            Quaternion.Euler(0, 0, angle),
            Time.deltaTime * rotationSpeed);
    }
}
