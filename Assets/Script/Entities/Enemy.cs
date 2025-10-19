using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Enemy : MonoBehaviour
{
    [Header("Stats")]
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
    }

    void Start()
    {
        // find player by tag (Player 오브젝트에 Tag=Player 설정 필수)
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
        OnDead?.Invoke(this);
        Destroy(gameObject);
    }

    // (옵션) 마우스 클릭으로 테스트용 데미지
    void OnMouseDown() => TakeDamage(1);
}