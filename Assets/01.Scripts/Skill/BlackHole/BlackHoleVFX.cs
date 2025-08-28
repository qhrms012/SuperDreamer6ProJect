using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHoleVFX : MonoBehaviour
{
    [SerializeField] private BlackHole source;   // 블랙홀 컴포넌트
    [SerializeField] private ParticleSystem psRing;
    [SerializeField] private ParticleSystem psSparks;

    [SerializeField] private float orbitalScale = 0.6f; // 회전 강도 스케일
    [SerializeField] private float radialScale = 0.45f; // 안쪽 빨려들기 스케일


    void LateUpdate()
    {
        if (!source) return;
        transform.position = source.transform.position;

        float r = source.Radius;

        // 반경 동기화(Shape.radius)
        SyncShapeRadius(psRing, r);
        SyncShapeRadius(psSparks, r + 0.3f);

        // 속도 스케일
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

