using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunState : Istate
{
    private StateMachine stateMachine;
    private Animator animator;
    private Player player;
    private AutoAimShooter shooter;

    public RunState(StateMachine machine, Animator animator, Player player, AutoAimShooter shooter)
    {
        stateMachine = machine;
        this.animator = animator;
        this.player = player;
        this.shooter = shooter;
    }

    public void Enter()
    {
        shooter.enabled = false;
        animator.Play("Run");
    }

    public void Execute(Vector2 playerVector)
    {
        if(playerVector.magnitude == 0)
        {
            stateMachine.SetState(new PlayerShotState(stateMachine, animator, player, shooter));
        }
    }

    public void Exit()
    {
        
    }
}
