using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyPatrol : MonoBehaviour
{
    [SerializeField] private float speed = 2.0f;
    [SerializeField] private float width = 3.0f;
    [SerializeField] private bool useRigidbody = true;

    private Rigidbody2D rb;
    private Vector2 leftPos, rightPos;
    private int dir = 1;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        float x = transform.position.x;
        float y = transform.position.y;
        leftPos = new Vector2(x - width * 0.5f, y);
        rightPos = new Vector2(x + width * 0.5f, y);
    }

    private void OnDisable()
    {
        rb.velocity = new Vector2(0f, rb.velocity.y);
    }

    void FixedUpdate()
    {
        Vector2 pos = useRigidbody && rb ? rb.position : (Vector2)transform.position;

        pos.x += dir * speed * Time.fixedDeltaTime;

        if (pos.x > rightPos.x) { pos.x = rightPos.x; dir = -1; }
        if (pos.x < leftPos.x) { pos.x = leftPos.x; dir = 1; }

        if (useRigidbody && rb) rb.MovePosition(pos);
        else transform.position = pos;
    }
}

