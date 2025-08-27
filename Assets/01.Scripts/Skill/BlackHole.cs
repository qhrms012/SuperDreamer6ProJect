using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BlackHole : MonoBehaviour
{
    [Header("Effect")]
    [SerializeField] private float radius = 3.5f;
    [SerializeField] private float pullForce = 20f;   // 당기는 세기
    [SerializeField] private float dps = 0f;          
    [SerializeField] private float duration = 3f;     // 3초

    [Header("Targeting")]
    [SerializeField] private LayerMask enemyMask;     // Enemy 레이어

    float endTime;
    Coroutine dmgCo;


    public void Activate(float dur, LayerMask mask)
    {
        duration = dur;
        enemyMask = mask;
    }

    void OnEnable()
    {
        if (enemyMask.value == 0) enemyMask = LayerMask.GetMask("Enemy");
        endTime = Time.time + duration;

        if (dps > 0f) dmgCo = StartCoroutine(CoDamageTick());
    }

    void OnDisable()
    {
        if (dmgCo != null) StopCoroutine(dmgCo);
        dmgCo = null;
    }

    void Update()
    {
        // 반경 내 적 끌어당기기
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius, enemyMask);
        foreach (var h in hits)
        {
            if (!h) continue;

            // 리지드바디가 있으면 물리 힘으로 끌기
            var rb = h.attachedRigidbody;
            Vector2 dir = ((Vector2)transform.position - (Vector2)h.transform.position);

            if (rb != null && rb.bodyType == RigidbodyType2D.Dynamic)
            {
                rb.AddForce(dir.normalized * pullForce, ForceMode2D.Force);
            }
            else
            {
                // RB가 없거나 Kinematic이면 트랜스폼을 조금씩 끌어오기
                h.transform.position += (Vector3)(dir.normalized * (pullForce * 0.5f * Time.deltaTime));
            }
        }

        // 지속시간 종료 → 풀로 반환(비활성화)
        if (Time.time >= endTime)
            gameObject.SetActive(false);
    }

    IEnumerator CoDamageTick()
    {
        var wait = new WaitForSeconds(0.2f);
        float tick = dps * 0.2f;
        while (Time.time < endTime)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius, enemyMask);
            foreach (var h in hits)
                if (h.TryGetComponent<IDamageable>(out var t)) t.TakeDamage(tick);
            yield return wait;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}

