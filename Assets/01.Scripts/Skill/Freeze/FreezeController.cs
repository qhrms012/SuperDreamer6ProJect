using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FreezeController : MonoBehaviour
{
    public bool IsFrozen { get; private set; }

    Rigidbody2D rb;
    Animator anim;

    RigidbodyConstraints2D originalConstraints;
    float originalAnimSpeed = 1f;
    float freezeEndTime;
    Coroutine co;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        if (anim) originalAnimSpeed = anim.speed;
        if (rb) originalConstraints = rb.constraints;
    }

    public void ApplyFreeze(float seconds)
    {
        // 중첩 시 갱신(연장)
        freezeEndTime = Mathf.Max(freezeEndTime, Time.time + seconds);
        if (co == null) co = StartCoroutine(CoFreeze());
    }

    IEnumerator CoFreeze()
    {
        IsFrozen = true;

        if (rb)
        {
            rb.velocity = Vector2.zero;
            originalConstraints = rb.constraints;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }
        if (anim)
        {
            originalAnimSpeed = anim.speed;
            anim.speed = 0f;
        }

        while (Time.time < freezeEndTime)
            yield return null;

        if (rb) 
            rb.constraints = originalConstraints;

        if (anim) 
            anim.speed = originalAnimSpeed;

        IsFrozen = false;
        co = null;
    }
}

