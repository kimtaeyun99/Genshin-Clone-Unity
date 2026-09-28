using UnityEngine;

public class PlayerDodgeState : PlayerStateBase
{
    private float timer;
    private Vector3 dodgeDirection;

    public PlayerDodgeState(
        PlayerController player,
        PlayerStateMachine stateMachine)
        : base(player, stateMachine)
    {
    }

    public override void Enter()
    {
        InputManager.Instance.OnAttack += HandleAttack;

        timer = 0f;

        Vector2 input = InputManager.Instance.MoveInput;

        if (input.sqrMagnitude > 0.01f)
        {
            dodgeDirection =
                player.Movement.GetMoveDirection(input);
        }
        else
        {
            dodgeDirection = player.transform.forward;
        }
    }

    public override void Update()
    {
        timer += Time.deltaTime;

        player.Movement.Dodge(dodgeDirection);

        if (timer >= player.Movement.DodgeDuration)
        {
            ChangeNextState();
        }
    }

    public override void Exit()
    {
        InputManager.Instance.OnAttack -= HandleAttack;
    }

    private void ChangeNextState()
    {
        if (InputManager.Instance.MoveInput.sqrMagnitude > 0.01f)
        {
            stateMachine.ChangeState(PlayerStateType.Move);
        }
        else
        {
            stateMachine.ChangeState(PlayerStateType.Idle);
        }
    }
    private void HandleAttack()
    {
        if(player.Movement.IsGrounded)
        {
            stateMachine.ChangeState(PlayerStateType.Attack);
        }
    }
}