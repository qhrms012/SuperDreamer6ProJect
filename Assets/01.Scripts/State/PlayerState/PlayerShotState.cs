using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShotState : Istate
{

    private StateMachine stateMachine;
    //private Animator animator;
    private Player player;
    private AutoAimShooter shooter;

    public PlayerShotState(StateMachine machine, /*Animator animator,*/ Player player, AutoAimShooter shooter)
    {
        stateMachine = machine;
        //this.animator = animator;
        this.player = player;
        this.shooter = shooter;
    }

    public void Enter()
    {
        Debug.Log("µé¾î¿È");
        shooter.enabled = true;
    }

    public void Execute(Vector2 playerVector)
    {
        
        if(playerVector.magnitude > 0)
        {
            stateMachine.SetState(new RunState(stateMachine, player, shooter));
        }
    }

    public void Exit()
    {
        
    }
}
