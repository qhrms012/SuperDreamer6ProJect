using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldSkill : MonoBehaviour, ISkill
{
    [SerializeField] private Transform owner;         // ���� Player transform
    [SerializeField] private ShieldBubble shield;     // �ڽ� ������Ʈ(�Ʒ� ��ũ��Ʈ ����)
    [SerializeField] private float duration = 3f;
    [SerializeField] private float cooldown = 6f;

    // ��/���̾� ����
    [SerializeField] private bool isPlayer = true;    // ���̸� false
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

        // ���� �´� ���̾�� ����
        int layer = LayerMask.NameToLayer(isPlayer ? playerShieldLayer : enemyShieldLayer);
        shield.gameObject.layer = layer;

        shield.Activate(owner, duration);

        lastUse = Time.time;
        return true;
    }
}

