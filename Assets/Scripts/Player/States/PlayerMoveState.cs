using UnityEngine;

public class PlayerMoveState : PlayerStateBase
{
    private float sprintHoldTimer;

    public PlayerMoveState(PlayerController player,PlayerStateMachine stateMachine) : base(player, stateMachine)
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
        if(player.Movement.CheckClimbableWall(out _) && InputManager.Instance.MoveInput.y > 0f)
        {
            stateMachine.ChangeState(PlayerStateType.Climb);
            return;
        }

        if (!player.Movement.IsGrounded)
        {
            stateMachine.ChangeState(PlayerStateType.Fall);
            return;
        }

        Vector2 moveInput = InputManager.Instance.MoveInput;

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

            player.Stamina.ConsumeStamina(player.Stamina.DashStaminaCostPerSecond);

            if (player.Stamina.IsEmpty)
            {
                player.Movement.ExitDashMode();
            }

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
        if (player.Movement.IsDashMode)
        {
            return;
        }
        if (player.Stamina.IsEmpty)
        {
            sprintHoldTimer = 0f;

        }
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