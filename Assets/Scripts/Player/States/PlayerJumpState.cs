public class PlayerJumpState : PlayerStateBase
{
    public PlayerJumpState(PlayerController player, StateMachine<PlayerStateType> stateMachine) : base(player,stateMachine)
    {

    }
    public override void Enter()
    {
        player.Movement.Jump();
        player.PlayerAnimationController.PlayJump();
    }
    public override void Update()
    {
        player.Movement.AirMove(InputManager.Instance.MoveInput);

        if(player.Movement.VerticalVelocity <= 0f)
        {
            stateMachine.ChangeState(PlayerStateType.Fall);
        }
    }
}
