using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MultiShotSkill : MonoBehaviour, ISkill
{
    [SerializeField] private TargetRef targetRef;
    [SerializeField] private Transform firePoint;
    [SerializeField] private int shots = 5;
    [SerializeField] private float spreadDeg = 30f;
    [SerializeField] private float cooldown = 5f;

    [Header("Ballistic")]
    [SerializeField] private float speedHint = 12f;
    [SerializeField] private float minT = 0.2f, maxT = 0.9f;

    private float lastUse = -999f;

    public float Cooldown => cooldown;
    public float GetRemainingCooldown() => Mathf.Max(0f, lastUse + cooldown - Time.time);

    public bool TryCast()
    {
        if (Time.time < lastUse + cooldown) return false;
        var tgt = targetRef ? targetRef.Target : null;
        if (!tgt) return false;

        // Áß¾Ó Åº v0 ±¸ÇÏ°í ÁÂ¿ì·Î È¸Àü
        Vector2 origin = firePoint.position;
        Vector2 dest = tgt.position;

        var sample = ObjectPoolManager.Instance.Get(PoolKey.PlayerBullet);
        var rbS = sample.GetComponent<Rigidbody2D>();
        float g = Physics2D.gravity.y * (rbS ? rbS.gravityScale : 1f);
        sample.SetActive(false);

        Vector2 v0Center = ComputeV0(origin, dest, g);
        int n = Mathf.Max(1, shots);
        for (int i = 0; i < n; i++)
        {
            float t = (n == 1) ? 0.5f : (float)i / (n - 1);
            float off = Mathf.Lerp(-spreadDeg * 0.5f, spreadDeg * 0.5f, t);
            Vector2 v0 = Rotate(v0Center, off);

            var go = ObjectPoolManager.Instance.Get(PoolKey.PlayerBullet);
            go.transform.position = origin;
            go.transform.up = v0.normalized;
            go.GetComponent<Bullet>()?.Initialize(BulletTeam.Player, v0);
        }

        lastUse = Time.time;
        return true;
    }

    Vector2 ComputeV0(Vector2 origin, Vector2 dest, float gy)
    {
        Vector2 dp = dest - origin;
        float T = Mathf.Clamp(dp.magnitude / Mathf.Max(0.01f, speedHint), minT, maxT);
        return new Vector2(dp.x / T, (dp.y - 0.5f * gy * T * T) / T);
    }
    Vector2 Rotate(Vector2 v, float deg)
    {
        float r = deg * Mathf.Deg2Rad; float c = Mathf.Cos(r), s = Mathf.Sin(r);
        return new Vector2(v.x * c - v.y * s, v.x * s + v.y * c);
    }
}


