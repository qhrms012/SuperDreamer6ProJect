using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldBubble : MonoBehaviour
{
    [SerializeField] private Collider2D col;     // ���� �ݶ��̴�
    [SerializeField] private GameObject vfxRoot; // ��ƼŬ/���־�

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
        // ���� �߽ɿ� ��ġ
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

