using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BlackHole : MonoBehaviour
{
    [Header("Effect")]
    [SerializeField] private float radius = 3.5f;
    [SerializeField] private float pullForce = 20f;   // ���� ����
    [SerializeField] private float dps = 0f;          
    [SerializeField] private float duration = 3f;     // 3��

    [Header("Targeting")]
    [SerializeField] private LayerMask enemyMask;     // Enemy ���̾�

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
        // �ݰ� �� �� �������
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius, enemyMask);
        foreach (var h in hits)
        {
            if (!h) continue;

            // ������ٵ� ������ ���� ������ ����
            var rb = h.attachedRigidbody;
            Vector2 dir = ((Vector2)transform.position - (Vector2)h.transform.position);

            if (rb != null && rb.bodyType == RigidbodyType2D.Dynamic)
            {
                rb.AddForce(dir.normalized * pullForce, ForceMode2D.Force);
            }
            else
            {
                // RB�� ���ų� Kinematic�̸� Ʈ�������� ���ݾ� �������
                h.transform.position += (Vector3)(dir.normalized * (pullForce * 0.5f * Time.deltaTime));
            }
        }

        // ���ӽð� ���� �� Ǯ�� ��ȯ(��Ȱ��ȭ)
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

