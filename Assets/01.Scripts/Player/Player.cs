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


    [Header("Render")]
    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [Header("HP")]
    public float maxHp = 100f;
    [SerializeField] private float curHp = 100f;

    private BoxCollider2D boxCollider;


    private StateMachine stateMachine;
    [Header("Combat")]
    [SerializeField] private AutoAimShooter shooter;

    public static event Action<float> onHpChanged;

    public Animator animator;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        stateMachine = new StateMachine();

        curHp = maxHp;
    }

    private void Start()
    {
        stateMachine.SetState(new PlayerShotState(stateMachine, animator, this,shooter));
    }

    private void Update()
    {
        stateMachine.Update(moveInput);
    }
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
        UpdateFacing();
    }
    private void UpdateFacing()
    {
        const float deadzone = 0.01f;
        if (moveInput.x > deadzone) spriteRenderer.flipX = false; // ¿À¸¥ÂÊ(D)
        if (moveInput.x < -deadzone) spriteRenderer.flipX = true;  // ¿ÞÂÊ(A)
        else                         spriteRenderer.flipX = false;
    }

    private void FixedUpdate()
    {
        Vector2 vector2 = moveInput.normalized * speed * Time.deltaTime;

        rb.MovePosition(rb.position + vector2);
    }

    public void TakeDamage(float damage)
    {
        curHp -= damage;
        onHpChanged?.Invoke(curHp);
    }
}
