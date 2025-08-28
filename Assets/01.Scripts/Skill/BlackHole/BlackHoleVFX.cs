using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHoleVFX : MonoBehaviour
{
    [SerializeField] private BlackHole source;   // ��Ȧ ������Ʈ
    [SerializeField] private ParticleSystem psRing;
    [SerializeField] private ParticleSystem psSparks;

    [SerializeField] private float orbitalScale = 0.6f; // ȸ�� ���� ������
    [SerializeField] private float radialScale = 0.45f; // ���� ������� ������


    void LateUpdate()
    {
        if (!source) return;
        transform.position = source.transform.position;

        float r = source.Radius;

        // �ݰ� ����ȭ(Shape.radius)
        SyncShapeRadius(psRing, r);
        SyncShapeRadius(psSparks, r + 0.3f);

        // �ӵ� ������
        float radial = -Mathf.Max(0f, source.PullAccel) * radialScale; 
        float orbital = Mathf.Max(0f, source.PullAccel) * orbitalScale;

        SetRadialAndOrbital(psRing, radial, orbital);
        SetRadialAndOrbital(psSparks, radial * 1.2f, orbital * 0.6f);


    }

    void SyncShapeRadius(ParticleSystem ps, float radius)
    {
        if (!ps) return;
        var sh = ps.shape;
        sh.radius = radius;
    }

    void SetRadialAndOrbital(ParticleSystem ps, float radial, float orbital)
    {
        if (!ps) return;
        var vol = ps.velocityOverLifetime;

        vol.radial = new ParticleSystem.MinMaxCurve(radial);
        vol.orbitalZ = new ParticleSystem.MinMaxCurve(orbital);
        vol.space = ParticleSystemSimulationSpace.Local;
    }

}

