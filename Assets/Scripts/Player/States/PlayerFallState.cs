public class PlayerFallState : PlayerStateBase
{
    public PlayerFallState(PlayerController player,PlayerStateMachine stateMachine) : base(player,stateMachine)
    {

    }
    public override void Enter()
    {
        
    }
    public override void Update()
    {
        player.Movement.AirMove(InputManager.Instance.MoveInput);

        if(player.Movement.IsGrounded)
        {
            if(InputManager.Instance.MoveInput.sqrMagnitude >= 0.01f)
            {
                stateMachine.ChangeState(PlayerStateType.Move);
            }
            else
            {
                stateMachine.ChangeState(PlayerStateType.Idle);
            }
        }
    }
    public override void Exit()
    {
        
    }
}
