using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int hp = 3;          // enemy health
    public int scoreValue = 10; // score added when killed

    public void TakeDamage(int dmg)
    {
        hp -= dmg;
        if (hp <= 0) OnDeath();
    }

    private void OnDeath()
    {
        GameManager.Instance.AddScore(scoreValue); // +10 score
        Destroy(gameObject);
    }

    // TEMP: quick test (press L)
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L)) TakeDamage(1);
    }
}