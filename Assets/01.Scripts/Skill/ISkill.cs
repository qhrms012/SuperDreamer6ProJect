using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISkill
{
    bool TryCast();                 // ���� ���� �� true
    float Cooldown { get; }         // ��ٿ�(��)
    float GetRemainingCooldown();   // ���� ��Ÿ��(��)
}

