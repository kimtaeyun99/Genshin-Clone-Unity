public class PlayerIdleState : PlayerStateBase
{
    public PlayerIdleState(
        PlayerController player,
        PlayerStateMachine stateMachine)
        : base(player, stateMachine)
    {
    }

    public override void Enter()
    {
        // Idle에 들어왔다는 것은 이동이 끊긴 것이므로
        // DashMode 종료
        player.Movement.ExitDashMode();

        InputManager.Instance.OnJump += HandleJump;
        InputManager.Instance.OnDodge += HandleDodge;
    }

    public override void Update()
    {
        if (!player.Movement.IsGrounded)
        {
            stateMachine.ChangeState(PlayerStateType.Fall);
            return;
        }

        if (InputManager.Instance.MoveInput.sqrMagnitude > 0.01f)
        {
            stateMachine.ChangeState(PlayerStateType.Move);
        }
    }

    public override void Exit()
    {
        InputManager.Instance.OnJump -= HandleJump;
        InputManager.Instance.OnDodge -= HandleDodge;
    }

    private void HandleJump()
    {
        if (player.Movement.IsGrounded)
        {
            stateMachine.ChangeState(PlayerStateType.Jump);
        }
    }

    private void HandleDodge()
    {
        if (player.Movement.IsGrounded)
        {
            stateMachine.ChangeState(PlayerStateType.Dodge);
        }
    }
}