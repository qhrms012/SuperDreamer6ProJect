using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiShotSkill : MonoBehaviour, ISkill
{
    [Header("Refs")]
    [SerializeField] private TargetRef targetRef;
    [SerializeField] private Transform firePoint;

    [Header("Volley")]
    [SerializeField] private int shots = 5;
    [SerializeField] private float spreadDeg = 30f;
    [SerializeField] private float cooldown = 5f;

    [Header("Ballistic")]
    [SerializeField] private float speedHint = 12f;
    [SerializeField] private float minT = 0.2f, maxT = 0.9f;

    [Header("Bullet")]
    [SerializeField] private PoolKey bulletKey = PoolKey.PlayerBullet;
    [SerializeField] private BulletTeam team = BulletTeam.Player;      

    private float lastUse = -999f;

    public float Cooldown => cooldown;
    public float GetRemainingCooldown() => Mathf.Max(0f, lastUse + cooldown - Time.time);

    public bool TryCast()
    {
        if (Time.time < lastUse + cooldown) return false;

        var tgt = targetRef.Target;
        firePoint = firePoint.transform;


        var sample = ObjectPoolManager.Instance.Get(bulletKey);
        var rbS = sample.GetComponent<Rigidbody2D>();
        float g = Physics2D.gravity.y * (rbS ? rbS.gravityScale : 1f);
        sample.SetActive(false);


        Vector2 origin = firePoint.position;
        Vector2 dest = tgt.position;
        Vector2 v0Center = ComputeV0(origin, dest, g);

        int n = Mathf.Max(1, shots);
        for (int i = 0; i < n; i++)
        {
            if (bulletKey == PoolKey.PlayerBullet)
            {
                AudioManager.Instance.PlaySfx(AudioManager.Sfx.Fire);
            }
            else if (bulletKey == PoolKey.EnemyBullet)
            {
                AudioManager.Instance.PlaySfx(AudioManager.Sfx.EnemyRaser);
            }
            float t = (n == 1) ? 0.5f : (float)i / (n - 1);
            float off = Mathf.Lerp(-spreadDeg * 0.5f, spreadDeg * 0.5f, t);
            Vector2 v0 = Rotate(v0Center, off);

            var go = ObjectPoolManager.Instance.Get(bulletKey);
            go.transform.position = origin;
            go.transform.up = v0.normalized;

            var b = go.GetComponent<Bullet>();
            var rb = go.GetComponent<Rigidbody2D>();
            if (b != null) b.Initialize(team, v0);
            else { if (rb) { rb.velocity = v0; go.SetActive(true); } }
        }

        lastUse = Time.time;
        return true;
    }

    Vector2 ComputeV0(Vector2 origin, Vector2 dest, float gy)
    {
        Vector2 dp = dest - origin;
        float T = Mathf.Clamp(dp.magnitude / Mathf.Max(0.01f, speedHint), minT, maxT);
        return new Vector2(
            dp.x / T,
            (dp.y - 0.5f * gy * T * T) / T
        );
    }

    Vector2 Rotate(Vector2 v, float deg)
    {
        float r = deg * Mathf.Deg2Rad; float c = Mathf.Cos(r), s = Mathf.Sin(r);
        return new Vector2(v.x * c - v.y * s, v.x * s + v.y * c);
    }
}



