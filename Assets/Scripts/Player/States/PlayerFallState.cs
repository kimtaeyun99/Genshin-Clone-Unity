public class PlayerFallState : PlayerStateBase
{
    public PlayerFallState(PlayerController player,StateMachine<PlayerStateType> stateMachine) : base(player,stateMachine)
    {

    }
    public override void Update()
    {
        player.Movement.AirMove(InputManager.Instance.MoveInput);

        if (player.Movement.IsGrounded)
        {
            ChangeNextState();
        }
    }
}
