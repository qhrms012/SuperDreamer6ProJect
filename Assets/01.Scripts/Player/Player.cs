using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour , IDamageable
{

    [Header("Move")]
    [SerializeField] private float speed = 6f;
    private Vector2 moveInput;
    private Rigidbody2D rb;

    [Header("HP")]
    [SerializeField] private float maxHp = 100f;
    [SerializeField] private float curHp = 100f;

    private BoxCollider2D boxCollider;


    private StateMachine stateMachine;

    public Action<float> onDamage;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        stateMachine = new StateMachine();

        curHp = maxHp;
    }

    private void Start()
    {
        stateMachine.SetState(new PlayerShotState(stateMachine, this));
    }
    public void OnMove(InputValue value)
    {
        stateMachine.SetState(new RunState(stateMachine, this));
        moveInput = value.Get<Vector2>();
    }

    private void FixedUpdate()
    {
        Vector2 vector2 = moveInput.normalized * speed * Time.deltaTime;

        rb.MovePosition(rb.position + vector2);
    }

    public void TakeDamage(float damage)
    {
        curHp -= damage;  
        onDamage?.Invoke(curHp);
    }
}
