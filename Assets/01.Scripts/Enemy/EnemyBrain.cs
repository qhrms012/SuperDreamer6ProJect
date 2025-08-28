using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyBrain : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private TargetRef targetRef;       // �÷��̾�
    [SerializeField] private AutoAimShooter shooter;
    [SerializeField] private EnemyPatrol patrol;        // �¿� �̵�

    [Header("Skills (ISkill)")]
    [SerializeField] private MonoBehaviour blackHoleSkill;  // ISkill
    [SerializeField] private MonoBehaviour multiShotSkill;  // ISkill
    [SerializeField] private MonoBehaviour freezeSkill;     // ISkill
    [SerializeField] private MonoBehaviour sheildSkill;     // ISkill

    [Header("Phase Durations (sec)")]
    [SerializeField] private Vector2 patrolPhase = new Vector2(2f, 3.5f);   // �¿� �̵�
    [SerializeField] private Vector2 shootPhase = new Vector2(1.2f, 2.2f); // ������
    [SerializeField] private float preCastPause = 0.25f;                  // ��ų �� ª�� ������
    [SerializeField] private float postCastPause = 0.35f;                 // ��ų �� ª�� ������

    [Header("Skill Selection")]
    [SerializeField] private bool useWeightedRandom = true;
    [SerializeField] private float weightBlackHole = 1f;
    [SerializeField] private float weightMultiShot = 1f;
    [SerializeField] private float weightFreeze = 1f;
    [SerializeField] private float weightShield = 1f;
    [SerializeField] private bool avoidRepeat = true;  // ���� ��ų �ݺ� ȸ��

    private ISkill sBlackHole, sMultiShot, sFreeze, sSheild;
    private ISkill lastSkill;

    void Awake()
    {
        if (!targetRef) targetRef = FindObjectOfType<TargetRef>();
        sBlackHole = blackHoleSkill as ISkill;
        sMultiShot = multiShotSkill as ISkill;
        sFreeze = freezeSkill as ISkill;
        sSheild = sheildSkill as ISkill;
    }

    void OnEnable()
    {
        StartCoroutine(CoBrain());
    }

    void OnDisable()
    {
        StopAllCoroutines();
        TogglePatrol(false);
        ToggleShoot(false);
    }

    IEnumerator CoBrain()
    {
        while (true)
        {
            // 1) �¿� �̵� ������
            ToggleShoot(false);
            TogglePatrol(true);
            yield return new WaitForSeconds(Random.Range(patrolPhase.x, patrolPhase.y));

            // 2) ���� ��� ������
            TogglePatrol(false);
            ToggleShoot(true);
            yield return new WaitForSeconds(Random.Range(shootPhase.x, shootPhase.y));

            // 3) ��ų ������
            ToggleShoot(false);
            yield return new WaitForSeconds(preCastPause);

            var skill = PickSkillAvailable();
            if (skill != null)
            {
                bool casted = skill.TryCast();
                if (casted) lastSkill = skill;
            }
            yield return new WaitForSeconds(postCastPause);
        }
    }

    void ToggleShoot(bool on)
    {
        if (shooter) shooter.enabled = on;

        var rb = patrol.GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(0f, rb.velocity.y);
    }

    void TogglePatrol(bool on)
    {
        if (patrol) patrol.enabled = on;

        if (!on)
        {
            var rb = patrol.GetComponent<Rigidbody2D>();
            rb.velocity = new Vector2(0f, rb.velocity.y);
        }
        
    }

    ISkill PickSkillAvailable()
    {
        // ��ٿ��� ���� �ĺ��� ������
        var candidates = new List<(ISkill s, float w)>();

        void Add(ISkill s, float w)
        {
            if (s == null) return;
            if (avoidRepeat && lastSkill == s) return;
            if (s.GetRemainingCooldown() <= 0f) candidates.Add((s, Mathf.Max(0f, w)));
        }

        if (useWeightedRandom)
        {
            Add(sBlackHole, weightBlackHole);
            Add(sMultiShot, weightMultiShot);
            Add(sFreeze, weightFreeze);
            Add(sSheild, weightShield);
            if (candidates.Count == 0)
            {
                // ��� ��ٿ��̸� �ƹ��ų�(��ª�� ��) �õ�
                ISkill fallback = EarliestReady(sBlackHole, sMultiShot, sFreeze);
                return fallback;
            }
            // ���� ����
            float total = 0f; foreach (var c in candidates) total += c.w;
            float r = Random.value * Mathf.Max(0.0001f, total);
            float acc = 0f;
            foreach (var c in candidates)
            {
                acc += c.w;
                if (r <= acc) return c.s;
            }
            return candidates[0].s;
        }
        else
        {
            
            if (IsReady(sMultiShot)) return sMultiShot;
            if (IsReady(sBlackHole)) return sBlackHole;           
            if (IsReady(sFreeze)) return sFreeze;
            if (IsReady(sSheild)) return sSheild;
            return EarliestReady(sMultiShot, sBlackHole, sFreeze, sSheild);
        }
    }

    bool IsReady(ISkill s) => s != null && s.GetRemainingCooldown() <= 0f;

    ISkill EarliestReady(params ISkill[] list)
    {
        ISkill best = null; float bestRemain = float.MaxValue;
        foreach (var s in list)
        {
            if (s == null) continue;
            float r = s.GetRemainingCooldown();
            if (r < bestRemain) { bestRemain = r; best = s; }
        }
        return best;
    }
}

