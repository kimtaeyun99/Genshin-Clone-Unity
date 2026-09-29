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
        player.Animation.PlayIdle();

        InputManager.Instance.OnJump += HandleJump;
        InputManager.Instance.OnDodge += HandleDodge;
        InputManager.Instance.OnAttack += HandleAttack;
        InputManager.Instance.OnSkill += HandleSkill;
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
        InputManager.Instance.OnAttack -= HandleAttack;
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
    private void HandleAttack()
    {
        if(player.Movement.IsGrounded)
        {
            stateMachine.ChangeState(PlayerStateType.Attack);
        }
    }
    private void HandleSkill()
    {
        if(player.Movement.IsGrounded)
        {
            stateMachine.ChangeState(PlayerStateType.Skill);
        }
    }
}