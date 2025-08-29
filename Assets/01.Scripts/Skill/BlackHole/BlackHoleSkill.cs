using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BlackHoleSkill : MonoBehaviour, ISkill
{
    [SerializeField] private TargetRef targetRef;
    [SerializeField] private float cooldown = 6f;
    private float lastUse = -999f;

    [Header("Bullet")]
    [SerializeField] private PoolKey bulletKey = PoolKey.PlayerBullet;
    public float Cooldown => cooldown;
    public float GetRemainingCooldown() => Mathf.Max(0f, lastUse + cooldown - Time.time);

    public bool TryCast()
    {
        if (Time.time < lastUse + cooldown) return false;

        var tgt =  targetRef.Target;

        var bh = ObjectPoolManager.Instance.Get(bulletKey);
        bh.transform.position = tgt.position;
        bh.SetActive(true);

        lastUse = Time.time;
        return true;
    }
}


