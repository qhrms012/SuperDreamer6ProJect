using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldSkill : MonoBehaviour, ISkill
{
    [SerializeField] private Transform owner;         // 보통 Player transform
    [SerializeField] private ShieldBubble shield;     // 자식 오브젝트(아래 스크립트 붙은)
    [SerializeField] private float duration = 3f;
    [SerializeField] private float cooldown = 6f;

    // 팀/레이어 설정
    [SerializeField] private bool isPlayer = true;    // 적이면 false
    [SerializeField] private string playerShieldLayer = "PlayerShield";
    [SerializeField] private string enemyShieldLayer = "EnemyShield";

    private float lastUse = -999f;
    public float Cooldown => cooldown;
    public float GetRemainingCooldown() => Mathf.Max(0, lastUse + cooldown - Time.time);

    void Awake()
    {
        if (!owner) owner = transform;
        if (!shield) shield = GetComponentInChildren<ShieldBubble>(true);
    }

    public bool TryCast()
    {
        if (Time.time < lastUse + cooldown) return false;
        if (!shield) return false;

        // 팀에 맞는 레이어로 세팅
        int layer = LayerMask.NameToLayer(isPlayer ? playerShieldLayer : enemyShieldLayer);
        shield.gameObject.layer = layer;

        shield.Activate(owner, duration);

        lastUse = Time.time;
        return true;
    }
}

