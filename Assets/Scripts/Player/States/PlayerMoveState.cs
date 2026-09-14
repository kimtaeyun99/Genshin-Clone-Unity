using UnityEngine;

public class PlayerMoveState : PlayerStateBase
{
    private float sprintHoldTimer;

    public PlayerMoveState(
        PlayerController player,
        PlayerStateMachine stateMachine)
        : base(player, stateMachine)
    {
    }

    public override void Enter()
    {
        InputManager.Instance.OnJump += HandleJump;
        InputManager.Instance.OnDodge += HandleDodge;

        sprintHoldTimer = 0f;
    }

    public override void Update()
    {
        if (!player.Movement.IsGrounded)
        {
            stateMachine.ChangeState(PlayerStateType.Fall);
            return;
        }

        Vector2 moveInput = InputManager.Instance.MoveInput;

        // 방향키를 놓으면 Idle + DashMode 종료
        if (moveInput.sqrMagnitude <= 0.01f)
        {
            player.Movement.ExitDashMode();

            stateMachine.ChangeState(PlayerStateType.Idle);
            return;
        }

        UpdateDashMode();

        if (player.Movement.IsDashMode)
        {
            player.Movement.DashMove(moveInput);
        }
        else
        {
            player.Movement.Move(moveInput);
        }
    }

    public override void Exit()
    {
        InputManager.Instance.OnJump -= HandleJump;
        InputManager.Instance.OnDodge -= HandleDodge;
    }

    private void UpdateDashMode()
    {
        // 이미 대시 모드에 들어갔다면
        // Shift를 떼어도 계속 유지
        if (player.Movement.IsDashMode)
        {
            return;
        }

        // 아직 대시 모드가 아니라면
        // Shift 유지 시간 측정
        if (InputManager.Instance.IsSprintHeld)
        {
            sprintHoldTimer += Time.deltaTime;

            if (sprintHoldTimer >= player.Movement.DashHoldTime)
            {
                player.Movement.EnterDashMode();
            }
        }
        else
        {
            sprintHoldTimer = 0f;
        }
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