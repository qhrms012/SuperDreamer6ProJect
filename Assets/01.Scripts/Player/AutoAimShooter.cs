using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoAimShooter : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private TargetRef targetRef;
    [SerializeField] private Transform firePoint;

    [Header("Fire")]
    [SerializeField] private float fireRate = 3f;
    [SerializeField] private float range = 0f;
    [SerializeField] private bool useBallistic = true;

    [Header("Straight")]
    [SerializeField] private float straightSpeed = 16f;

    [Header("Ballistic")]
    [SerializeField] private float speedHint = 12f;
    [SerializeField] private float minT = 0.25f, maxT = 0.9f;
    [SerializeField] private float arcHeight = 1.5f;

    float shootTimer;

    void Awake()
    {
        firePoint = transform;
    }

    void Update()
    {
        var tgt = targetRef ? targetRef.Target : null;
        if (!tgt || !tgt.gameObject.activeInHierarchy) return;

        if (range > 0f)
        {
            float sq = ((Vector2)tgt.position - (Vector2)transform.position).sqrMagnitude;
            if (sq > range * range) return; // 사거리 밖이면 발사 안 함
        }

        shootTimer -= Time.deltaTime;
        if (shootTimer <= 0f)
        {
            Shoot(tgt);
            shootTimer = 1f / fireRate;
        }
    }

    void Shoot(Transform tgt)
    {
        if (useBallistic) ShootBallistic(tgt);
        else ShootStraight(tgt);
    }

    void ShootStraight(Transform tgt)
    {
        Vector2 origin = firePoint.position;
        Vector2 dir = ((Vector2)tgt.position - origin).normalized;

        FireVelocity(dir * straightSpeed);
    }

    void ShootBallistic(Transform tgt)
    {
        Vector2 origin = firePoint.position;
        Vector2 dest = tgt.position;

        var bullet = ObjectPoolManager.Instance.Get(PoolKey.PlayerBullet);
        var rbS = bullet.GetComponent<Rigidbody2D>();

        float g = Physics2D.gravity.y * (rbS ? rbS.gravityScale : 1f);
        float gAbs = Mathf.Abs(g);

        bullet.SetActive(false);

        Vector2 dp2 = dest - origin;
        float yApex = Mathf.Max(origin.y, dest.y) + Mathf.Max(0.01f, arcHeight);

        float hUp = Mathf.Max(0.001f, yApex - origin.y);
        float v0y = Mathf.Sqrt(2f * gAbs * hUp);
        float tUp = v0y / gAbs;

        float hDown = Mathf.Max(0f, yApex - dest.y);
        float tDown = Mathf.Sqrt(2f * hDown / gAbs);

        float T = Mathf.Clamp(tUp + tDown, minT, maxT);
        float v0x = dp2.x / Mathf.Max(0.001f, T);

        Vector2 v0 = new Vector2(v0x, v0y);
        FireVelocity(v0, orient: true);
    }

    void FireVelocity(Vector2 v0, bool orient = true)
    {
        var go = ObjectPoolManager.Instance.Get(PoolKey.PlayerBullet);
        go.transform.position = firePoint.position;

        if (orient) go.transform.up = v0.normalized;

        var b = go.GetComponent<Bullet>();
        var rb = go.GetComponent<Rigidbody2D>();

        if (b != null) 
            b.Initialize(BulletTeam.Player, v0);

        else 
        { 
            if (rb) 
            { 
                rb.velocity = v0; go.SetActive(true); 
            } 
        }
    }
}

