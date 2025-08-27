using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BulletTeam { Player, Enemy }
public interface IDamageable
{
    void TakeDamage(float amount);
}
public class Bullet : MonoBehaviour
{
    [Header("Core")]
    [SerializeField]    private float damage = 20f;
    [SerializeField]    private float speed = 2f;
    [SerializeField]    private BulletTeam team = BulletTeam.Player;

    private Rigidbody2D rb;
    private BoxCollider2D boxCollider2D;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (team == BulletTeam.Player && collision.CompareTag("Player")) return;
        if (team == BulletTeam.Enemy && collision.CompareTag("Enemy")) return;

        if(collision.TryGetComponent<IDamageable>(out var target))
        {
            target.TakeDamage(damage);
            DisableBullet();
        }

        if (collision.CompareTag("Wall"))
        {
            DisableBullet();
        }
    }



    private void DisableBullet()
    {
        gameObject.SetActive(false);
    }
}
