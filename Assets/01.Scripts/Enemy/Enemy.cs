using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour , IDamageable
{
    [SerializeField]
    private float curHp;
    [SerializeField]
    private float maxHp;

    private StateMachine stateMachine;
    
    
    private Rigidbody2D rb;

    public Action<float> onDamage;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        stateMachine = new StateMachine();
        curHp = maxHp;
    }

    private void Start()
    {
        
    }

    public void TakeDamage(float damage)
    {
        curHp -= damage;
        onDamage?.Invoke(curHp);
        Debug.Log($"EnemyÃ¼·Â : {curHp}");
    }
}
