using UnityEngine;

public class PlayerClimbState : PlayerStateBase 
{
    public PlayerClimbState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {

    }
    public override void Enter()
    {

    }
    public override void Update()
    {
        if(!player.Movement.CheckClimbableWall(out RaycastHit hit))
        {
            stateMachine.ChangeState(PlayerStateType.Fall);
            return;
        }

        Vector3 wallDirection = -hit.normal;
        wallDirection.y = 0f;

        player.transform.rotation = Quaternion.LookRotation(wallDirection);

        float verticalInput = InputManager.Instance.MoveInput.y;

        player.Movement.ClimbVertical(verticalInput);
    }
    public override void Exit()
    {
        
    }
}
