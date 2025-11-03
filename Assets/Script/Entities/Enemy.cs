using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    public int maxHP = 3;   
    public int hp = 3;
    public int scoreValue = 10;
    public float moveSpeed = 2.5f;

    [NonSerialized] public Action<Enemy> OnDead;

    Rigidbody2D rb;
    Transform player;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        hp = Mathf.Clamp(hp, 0, maxHP); 
    }

    void Start()
    {
        var p = GameObject.FindGameObjectWithTag("Player");
        if (p) player = p.transform;
    }

    void Update()
    {
        if (!player) return;
        Vector2 dir = (player.position - transform.position).normalized;
        rb.MovePosition(rb.position + dir * moveSpeed * Time.deltaTime);
    }

    public void TakeDamage(int dmg)
    {
        hp -= dmg;
        if (hp <= 0) Die();
    }

    void Die()
    {
        GameManager.Instance.AddScore(scoreValue);

        // --- Track total kills ---
        int kills = PlayerPrefs.GetInt("TotalKills", 0) + 1;
        PlayerPrefs.SetInt("TotalKills", kills);

        // Check for kill-based achievement unlock
        if (kills >= 5 && PlayerPrefs.GetInt("Achievement_5Kills", 0) == 0)
        {
            PlayerPrefs.SetInt("Achievement_5Kills", 1);
            Debug.Log("[Achievement] 5 Kills unlocked!");
        }

        OnDead?.Invoke(this);
        SFXManager.Instance?.PlayEnemyDie(); // play enemy death sfx
        Destroy(gameObject);
    }

    void OnMouseDown() => TakeDamage(1);
}