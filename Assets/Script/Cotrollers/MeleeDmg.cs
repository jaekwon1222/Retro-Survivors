using UnityEngine;
using System.Collections.Generic;

public class MeleeHitbox : MonoBehaviour
{
    public int damage = 1;
    private WeaponMeleeParent parent;

    private HashSet<Enemy> _alreadyHit = new HashSet<Enemy>();

    void Awake()
    {
        parent = GetComponentInParent<WeaponMeleeParent>();
    }

    public void BeginAttack()
    {
        _alreadyHit.Clear();
        gameObject.SetActive(true);
    }

    public void EndAttack()
    {
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //if (!parent.IsAttacking) return;

        var enemy = other.GetComponent<Enemy>();
        if (!enemy) return;

        if (_alreadyHit.Contains(enemy)) return; // prevents double-hit same swing
        _alreadyHit.Add(enemy);

        enemy.TakeDamage(damage);
    }
}
