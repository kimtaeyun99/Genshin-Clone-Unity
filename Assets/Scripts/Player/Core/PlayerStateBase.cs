using UnityEngine;

public abstract class PlayerStateBase
{
    protected PlayerController player;
    protected PlayerStateMachine stateMachine;

    protected PlayerStateBase(PlayerController player, PlayerStateMachine stateMachine)
    {
        this.player = player;
        this.stateMachine = stateMachine;
    }

    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void Exit() { }
}
