using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum BulletTeam { Player, Enemy }
public interface IDamageable { void TakeDamage(float amount); }

public class Bullet : MonoBehaviour
{
    [Header("Core")]
    [SerializeField] private float damage = 20f;
    [SerializeField] private float speed = 2f;               // �⺻ �ӵ�
    [SerializeField] private BulletTeam team = BulletTeam.Player;
    [SerializeField] private float lifetime = 3f;             // ����

    private Rigidbody2D rb;
    private BoxCollider2D boxCollider2D;

    private float timer;
    private bool velocityInjected;
    private Vector2 injectedVelocity;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();
    }

    private void OnEnable()
    {
        timer = lifetime;

        // �ʱ� �ӵ� ����
        if (velocityInjected)
        {
            rb.velocity = injectedVelocity;
            velocityInjected = false;
        }
        else
        {
            rb.velocity = (Vector2)transform.up * speed;
        }
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f) DisableBullet();
    }
    // Ǯ���� ���� �� ��/�ӵ��� ����
    public void Initialize(BulletTeam team, Vector2 velocity)
    {
        this.team = team;
        injectedVelocity = velocity;
        velocityInjected = true;

        if (gameObject.activeInHierarchy)
            rb.velocity = velocity;         // �̹� Ȱ�� ���¸� ��� �ݿ�
        else
            gameObject.SetActive(true);     // ��Ȱ�� ���¸� Ȱ��ȭ �� OnEnable���� ����
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ���� �� �ǰ� ����
        if (team == BulletTeam.Player && collision.CompareTag("Player")) return;
        if (team == BulletTeam.Enemy && collision.CompareTag("Enemy")) return;

        if (collision.TryGetComponent<IDamageable>(out var target))
        {
            target.TakeDamage(damage);

            // ����/ȭ�� �� ����Ʈ ȿ�� ȣ��
            var effects = GetComponents<IOnHitEffect>();
            for (int i = 0; i < effects.Length; i++)
                effects[i].OnHit(collision.gameObject);

            DisableBullet();
        }

        if (collision.CompareTag("Wall"))
        {
            DisableBullet();
        }
    }

    private void DisableBullet()
    {
        if (rb) 
            rb.velocity = Vector2.zero;
        gameObject.SetActive(false);
    }
}

public interface IOnHitEffect
{
    void OnHit(GameObject target);
}

