using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeSkill : MonoBehaviour, ISkill
{
    [SerializeField] private TargetRef targetRef;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float arrowSpeed = 18f;
    [SerializeField] private float cooldown = 5f;

    private float lastUse = -999f;
    public float Cooldown => cooldown;
    public float GetRemainingCooldown() => Mathf.Max(0f, lastUse + cooldown - Time.time);

    public bool TryCast()
    {
        if (Time.time < lastUse + cooldown) return false;
        var tgt = targetRef ? targetRef.Target : null;
        if (!tgt) return false;

        var go = ObjectPoolManager.Instance.Get(PoolKey.FreezeArrow);
        go.transform.position = firePoint.position;
        Vector2 dir = ((Vector2)tgt.position - (Vector2)firePoint.position).normalized;
        go.transform.up = dir;
        go.GetComponent<Rigidbody2D>().gravityScale = 0f;
        go.GetComponent<Bullet>()?.Initialize(BulletTeam.Player, dir * arrowSpeed);

        lastUse = Time.time;
        return true;
    }
}



