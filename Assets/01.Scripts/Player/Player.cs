using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{

    [SerializeField]
    private float speed;

    [SerializeField]
    private float curHp;

    public Vector2 playerPosition;

    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;


    private StateMachine stateMachine;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        stateMachine = new StateMachine();
    }

    private void Start()
    {
        stateMachine.SetState(new ShotState(stateMachine, this));
    }
    public void OnMove(InputValue value)
    {
        stateMachine.SetState(new RunState(stateMachine, this));
        playerPosition = value.Get<Vector2>();
    }

    private void FixedUpdate()
    {
        Vector2 vector2 = playerPosition.normalized * speed * Time.deltaTime;

        rb.MovePosition(rb.position + vector2);
    }
}
