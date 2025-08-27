using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TargetRef : MonoBehaviour
{
    [SerializeField] private Transform target;
    public Transform Target => target;

    public void Bind(Transform t) => target = t;
    public void Clear() => target = null;

    void Start()
    {
        if (!target)
        {
            var go = GameObject.FindWithTag("Enemy");
            if (go) target = go.transform;
        }
    }
}
