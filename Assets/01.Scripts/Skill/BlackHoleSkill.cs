using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BlackHoleSkill : MonoBehaviour, ISkill
{
    [SerializeField] private TargetRef targetRef;
    [SerializeField] private float cooldown = 6f;
    private float lastUse = -999f;

    public float Cooldown => cooldown;
    public float GetRemainingCooldown() => Mathf.Max(0f, lastUse + cooldown - Time.time);

    public bool TryCast()
    {
        if (Time.time < lastUse + cooldown) return false;

        var tgt = targetRef ? targetRef.Target : null;
        if (!tgt) return false;

        var bh = ObjectPoolManager.Instance.Get(PoolKey.BlackHole);
        bh.transform.position = tgt.position;
        // bh.GetComponent<BlackHole>()?.Activate(3f, LayerMask.GetMask("Enemy"));
        bh.SetActive(true);

        lastUse = Time.time;
        return true;
    }
}


