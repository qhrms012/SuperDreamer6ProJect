using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FreezeOnHitEffect : MonoBehaviour, IOnHitEffect
{
    [SerializeField] private float freezeDuration = 2f; // 맞은 적 2초 정지

    public void OnHit(GameObject target)
    {
        var f = target.GetComponent<FreezeController>();
        f.ApplyFreeze(freezeDuration);
    }
}

