using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Istate
{
    void Enter();
    void Execute(Vector2 playerVector);
    void Exit();
}
