using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldBubble : MonoBehaviour
{
    [SerializeField] private Collider2D col;     // 방패 콜라이더
    [SerializeField] private GameObject vfxRoot; // 파티클/비주얼

    Transform follow;
    float endTime;

    public void Activate(Transform owner, float duration)
    {
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.ShieldOn);
        follow = owner;
        endTime = Time.time + duration;
        if (col) col.enabled = true;
        if (vfxRoot) vfxRoot.SetActive(true);
        gameObject.SetActive(true);
        // 오너 중심에 위치
        transform.position = owner.position;
    }

    void LateUpdate()
    {
        if (!follow) return;
        transform.position = follow.position;
        if (Time.time >= endTime)
        {
            if (col) col.enabled = false;
            if (vfxRoot) vfxRoot.SetActive(false);
            gameObject.SetActive(false);
        }
    }
}

