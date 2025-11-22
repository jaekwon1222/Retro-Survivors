using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    public Collider2D hitbox;
    public float attackDuration = 0.15f;
    private bool attacking;

    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }


    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !attacking)
        {
            StartCoroutine(DoAttack());
        }
    }

    private System.Collections.IEnumerator DoAttack()
    {
        attacking = true;
        hitbox.enabled = true;   // turn on hitbox
        yield return new WaitForSeconds(attackDuration);
        hitbox.enabled = false;  // turn off hitbox
        attacking = false;
    }
}
