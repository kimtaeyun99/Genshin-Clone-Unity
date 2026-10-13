public abstract class PlayerStateBase : State
{
    protected PlayerController player;

    protected StateMachine<PlayerStateType> stateMachine;

    protected PlayerStateBase(PlayerController player, StateMachine<PlayerStateType> stateMachine)
    {
        this.player = player;
        this.stateMachine = stateMachine;
    }

    protected void ChangeNextState()
    {
        bool moving = InputManager.Instance.MoveInput.sqrMagnitude > 0.01f;

        if(moving)
        {
            stateMachine.ChangeState(PlayerStateType.Move);
        }
        else
        {
            stateMachine.ChangeState(PlayerStateType.Idle);
        }
    }
}
