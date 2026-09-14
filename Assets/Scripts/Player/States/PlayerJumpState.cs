using UnityEngine.Playables;

public class PlayerJumpState : PlayerStateBase
{
    public PlayerJumpState(PlayerController player, PlayerStateMachine stateMachine) : base(player,stateMachine)
    {

    }
    public override void Enter()
    {
        player.Movement.Jump();
    }
    public override void Update()
    {
        player.Movement.AirMove(InputManager.Instance.MoveInput);

        if(player.Movement.VerticalVelocity <= 0f)
        {
            stateMachine.ChangeState(PlayerStateType.Fall);
        }
    }
    public override void Exit()
    {
        
    }
}
