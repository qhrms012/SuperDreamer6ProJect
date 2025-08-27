using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class AutoAimShooter : MonoBehaviour
{
    [Header("Targeting")]
    [SerializeField] private float range = 6f;
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private float retargetInterval = 0.15f;

    [Header("Shooting")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 3f;

    // 포물선
    [Header("Ballistic")]
    [SerializeField] private float ballisticSpeedHint = 12f;
    [SerializeField] private float minFlightTime = 0.25f;
    [SerializeField] private float maxFlightTime = 0.9f;


    [SerializeField] private float arcHeight = 1.5f;

    [Header("Aiming (optional)")]
    [SerializeField] private Transform aimRoot;

    private Transform target;
    private float shootTimer;
    private float retargetTimer;

    private void Awake()
    {
        if (!firePoint) firePoint = transform;
        if (!aimRoot) aimRoot = transform;
        if (enemyMask.value == 0) enemyMask = LayerMask.GetMask("Enemy");
    }

    private void Update()
    {
        // 타겟 재탐색
        retargetTimer -= Time.deltaTime;
        if (retargetTimer <= 0f)
        {
            target = FindClosestTarget();
            retargetTimer = retargetInterval;
        }
        if (target == null || firePoint == null) return;

        // 조준
        //Vector2 aimDir = (target.position - firePoint.position).normalized;
        //if (aimRoot) aimRoot.up = aimDir;
        //firePoint.up = aimDir;

        // 발사
        shootTimer -= Time.deltaTime;
        if (shootTimer <= 0f)
        {
            ShootBallistic(target);
            shootTimer = 1f / fireRate;
        }
    }

    private Transform FindClosestTarget()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, range, enemyMask);
        Transform best = null;
        float bestSq = float.MaxValue;

        foreach (var h in hits)
        {
            if (!h || !h.gameObject.activeInHierarchy) continue;
            float sq = (h.transform.position - transform.position).sqrMagnitude;
            if (sq < bestSq) { bestSq = sq; best = h.transform; }
        }
        return best;
    }

    // 포물선 발사
    private void ShootBallistic(Transform tgt)
    {
        GameObject go = ObjectPoolManager.Instance.Get(PoolKey.PlayerBullet);
        go.transform.position = firePoint.position;

        var bullet = go.GetComponent<Bullet>();
        var rb = go.GetComponent<Rigidbody2D>();


        float g = Physics2D.gravity.y * rb.gravityScale;
        float gAbs = Mathf.Abs(g);

        Vector2 origin = firePoint.position;
        Vector2 dest = tgt.position;
        Vector2 dp = dest - origin;

        Vector2 v0;


        float yApex = Mathf.Max(origin.y, dest.y) + Mathf.Max(0.01f, arcHeight);

        // 올라가는 구간
        float hUp = Mathf.Max(0.001f, yApex - origin.y);
        float v0y = Mathf.Sqrt(2f * gAbs * hUp);   // 위로 가는 초기 y속도
        float tUp = v0y / gAbs;

        // 내려가는 구간
        float hDown = Mathf.Max(0f, yApex - dest.y);
        float tDown = Mathf.Sqrt(2f * hDown / gAbs);

        float T = Mathf.Clamp(tUp + tDown, minFlightTime, maxFlightTime);
        float v0x = dp.x / Mathf.Max(0.001f, T);

        v0 = new Vector2(v0x, v0y);


        // 총알 방향 연출
        go.transform.up = v0.normalized;

        // 초기 속도 적용

        bullet.Initialize(BulletTeam.Player, v0);

    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}

