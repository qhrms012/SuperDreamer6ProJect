using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private float radius = 3.5f;         // ���� �ݰ�
    [SerializeField] private float pullAccel = 28f;       // �߾����� ���� �ӵ�
    [SerializeField] private float duration = 3f;         // ���ӽð�
    [SerializeField] private LayerMask targetMask;        // ��� ���̾�(�÷��̾�/��)

    [Header("Swirl (ȸ��)")]
    [SerializeField] private float swirlAccel = 16f;
    [SerializeField, Range(-1, 1)] private int spin = 1;  // ȸ�� ����(+1 �ð� / -1 �ݽð�)
    [SerializeField] private float falloffPower = 1.6f;

    [Header("Core & Limits")]
    [SerializeField] private float coreRadius = 0.35f;
    [SerializeField] private float maxSpeed = 14f;        // �ִ� �ӵ�
    [SerializeField] private float coreDamp = 0.15f;

    [Header("Optional Damage")]
    [SerializeField] private float dps = 0f;
    [SerializeField] private float tickInterval = 0.2f;

    public float Radius => radius;
    public float PullAccel => pullAccel;

    private float endTime;
    private float tickTimer;

    public void Activate(float dur, LayerMask mask)
    {
        duration = dur;
        targetMask = mask;
    }

    private void OnEnable()
    {
        if (targetMask.value == 0) targetMask = LayerMask.GetMask("Enemy");

        AudioManager.Instance.PlaySfx(AudioManager.Sfx.BlackHole);
        endTime = Time.time + duration;
        tickTimer = tickInterval;
    }

    private void OnDisable()
    {
        // ����� ���� �ܿ� �ӵ� ����(�������� �̲����� ����)
        var hits = Physics2D.OverlapCircleAll(transform.position, radius, targetMask);
        foreach (var h in hits)
        {
            var rb = h ? h.attachedRigidbody : null;
            if (rb && rb.bodyType == RigidbodyType2D.Dynamic)
            {
                rb.velocity = Vector2.zero;
                rb.angularVelocity = 0f;
            }
        }
    }

    private void FixedUpdate()
    {
        var hits = Physics2D.OverlapCircleAll(transform.position, radius, targetMask);
        Vector2 c = transform.position;
        float dt = Time.fixedDeltaTime;
        float vmax2 = maxSpeed * maxSpeed;

        foreach (var h in hits)
        {
            if (!h || !h.gameObject.activeInHierarchy) continue;

            var rb = h.attachedRigidbody;
            Vector2 p = h.transform.position;
            Vector2 toC = c - p;
            float d = toC.magnitude;
            if (d < 1e-4f) continue;

            Vector2 dir = toC / d;
            Vector2 tan = new Vector2(-dir.y, dir.x)
                          * (spin >= 0 ? 1f : -1f);

            // �߽ɿ� �������� �� ���ϰ�
            float t = Mathf.Clamp01(1f - d / radius);
            float f = Mathf.Pow(t, falloffPower);

            if (rb && rb.bodyType == RigidbodyType2D.Dynamic)
            {
                rb.AddForce(dir * (pullAccel * f * rb.mass), ForceMode2D.Force);
                rb.AddForce(tan * (swirlAccel * f * rb.mass), ForceMode2D.Force);


                if (d < coreRadius)
                {
                    Vector2 v = rb.velocity;
                    float vRad = Vector2.Dot(v, dir);  
                    v -= dir * vRad;                    
                    v *= (1f - coreDamp);               
                    rb.velocity = v;

                    
                    rb.MovePosition(c - dir * coreRadius);
                }
                else
                {                   
                    if (rb.velocity.sqrMagnitude > vmax2)
                        rb.velocity = rb.velocity.normalized * maxSpeed;
                }
            }

        }

        //��Ʈ ������
        if (dps > 0f)
        {
            tickTimer -= dt;
            if (tickTimer <= 0f)
            {
                float dmg = dps * tickInterval;
                foreach (var h in hits)
                    if (h && h.TryGetComponent<IDamageable>(out var tComp)) tComp.TakeDamage(dmg);
                tickTimer += tickInterval;
            }
        }

        if (Time.time >= endTime)
            gameObject.SetActive(false);
    }

}





