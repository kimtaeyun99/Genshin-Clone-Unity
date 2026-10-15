using UnityEngine;

public class PlayerDodgeState : PlayerStateBase
{
    private float timer;
    private Vector3 dodgeDirection;

    public PlayerDodgeState(
        PlayerController player,
        StateMachine<PlayerStateType> stateMachine)
        : base(player, stateMachine)
    {
    }

    public override void Enter()
    {
        player.Stamina.ConsumeStaminaInstant(player.Stamina.DodgeStaminaCost);
        player.PlayerAnimationController.PlayDodge();
        player.Health.StartDodgeInvincible(player.Movement.DodgeDuration);

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
}