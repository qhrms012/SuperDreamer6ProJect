using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour , IDamageable
{
    [SerializeField]
    private float curHp;
    public float maxHp;

    private StateMachine stateMachine;
    
    
    private Rigidbody2D rb;

    public static event Action<float> onHpChanged;

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
        onHpChanged?.Invoke(curHp);
        Debug.Log($"Enemyü�� : {curHp}");
    }
}
