using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldHitVFX : MonoBehaviour
{
    [SerializeField] private ParticleSystem hitPrefab;
    [SerializeField] private Transform vfxParent;

    CircleCollider2D circle;

    void Awake()
    {
        circle = GetComponent<CircleCollider2D>();
        if (!vfxParent) vfxParent = transform;
    }

    void OnTriggerEnter2D(Collider2D other)
    {

        if (!hitPrefab) return;

        Vector2 bulletPos = other.bounds.center;
        Vector2 hitPos = circle ? circle.ClosestPoint(bulletPos)
                                : (Vector2)transform.position;

        var ps = Instantiate(hitPrefab, hitPos, Quaternion.identity, vfxParent);
        ps.Play();
        Destroy(ps.gameObject, 1.0f);
    }
}

