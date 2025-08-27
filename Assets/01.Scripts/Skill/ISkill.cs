using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISkill
{
    bool TryCast();                 // 시전 성공 시 true
    float Cooldown { get; }         // 쿨다운(초)
    float GetRemainingCooldown();   // 남은 쿨타임(초)
}

