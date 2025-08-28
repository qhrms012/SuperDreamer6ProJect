using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FreezeOnHitEffect : MonoBehaviour, IOnHitEffect
{
    [SerializeField] private float freezeDuration = 2f; // 맞은 적 2초 정지

    public void OnHit(GameObject target)
    {
        FreezeTintVisual tint;

        var freeze = target.GetComponentInParent<FreezeController>();
        tint = target.GetComponentInChildren<FreezeTintVisual>();

        // 2) 빙결 적용

        freeze.ApplyFreeze(freezeDuration);
        tint.Play(freezeDuration);

        
    }
}

