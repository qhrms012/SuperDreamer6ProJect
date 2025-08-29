using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldVFXBinder : MonoBehaviour
{
    [SerializeField] private CircleCollider2D shieldCollider; // PlayerShield의 콜라이더
    [SerializeField] private ParticleSystem psRim;
    [SerializeField] private ParticleSystem psField;
    [SerializeField] private float rimSpin = 14f; // OrbitalZ 목표값
    [SerializeField] private float fieldSpin = 6f;

    void Reset()
    {
        shieldCollider = GetComponentInParent<CircleCollider2D>();
    }

    void LateUpdate()
    {
        if (!shieldCollider) return;
        float r = shieldCollider.radius;

        SyncRadius(psRim, 1.5f);
        SyncRadius(psField, 1.5f);

        SetOrbital(psRim, rimSpin);
        SetOrbital(psField, fieldSpin);

        transform.position = shieldCollider.transform.position;
    }

    void SyncRadius(ParticleSystem ps, float r)
    {
        if (!ps) return;
        var sh = ps.shape;
        sh.radius = r;
    }

    void SetOrbital(ParticleSystem ps, float orbitalZ)
    {
        if (!ps) return;
        var vol = ps.velocityOverLifetime;
        vol.orbitalZ = new ParticleSystem.MinMaxCurve(orbitalZ);
        vol.space = ParticleSystemSimulationSpace.Local;
    }
}

