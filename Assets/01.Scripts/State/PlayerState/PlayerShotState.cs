using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShotState : Istate
{

    private StateMachine stateMachine;
    //private Animator animator;
    private Player player;

    public PlayerShotState(StateMachine machine, /*Animator animator,*/ Player player)
    {
        stateMachine = machine;
        //this.animator = animator;
        this.player = player;
    }

    public void Enter()
    {
        
    }

    public void Execute(Vector2 playerVector)
    {
        if(playerVector.magnitude > 0)
        {
            stateMachine.SetState(new RunState(stateMachine, player));
        }
    }

    public void Exit()
    {
        
    }
}
