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
        timer = 0f;

        Vector2 input = InputManager.Instance.MoveInput;

        // 방향키 + Shift
        // → 방향키 방향으로 Dodge
        if (input.sqrMagnitude > 0.01f)
        {
            dodgeDirection =
                player.Movement.GetMoveDirection(input);
        }
        // Shift만 입력
        // → 캐릭터가 바라보는 방향으로 Dodge
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

    private void ChangeNextState()
    {
        // Dodge 종료 시 방향키를 누르고 있으면 Move
        if (InputManager.Instance.MoveInput.sqrMagnitude > 0.01f)
        {
            stateMachine.ChangeState(PlayerStateType.Move);
        }
        // 방향키가 없으면 Idle
        else
        {
            stateMachine.ChangeState(PlayerStateType.Idle);
        }
    }
}