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
    
    public Animator animator;
    
    private Rigidbody2D rb;

    public static event Action<float> onHpChanged;
    public static event Action<bool> isDead;

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
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.EnemyHit);
        if (curHp < 0)
        {
            animator.Play("Dead");
            isDead?.Invoke(true);
            AudioManager.Instance.PlaySfx(AudioManager.Sfx.Win);
            AudioManager.Instance.PlaySfx(AudioManager.Sfx.EnemyDead);
        }
        Debug.Log($"EnemyÃ¼·Â : {curHp}");
    }
}
