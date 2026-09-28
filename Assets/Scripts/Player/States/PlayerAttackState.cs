using UnityEngine;

public class PlayerAttackState : PlayerStateBase
{
    private float timer;
    private float attackDuration = 0.5f;
    public PlayerAttackState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {

    }
    public override void Enter()
    {
        timer = 0f;

        player.Movement.ExitDashMode();

        player.CharacterManager.CurrentAnimator.SetTrigger("Attack");

        Debug.Log("Attack ÁøÀÔ");
    }
    public override void Update()
    {
        timer += Time.deltaTime;

        if(timer >= attackDuration)
        {
            ChangeNextState();
        }
    }
    public override void Exit()
    {
        Debug.Log("Attack ÅðÀå");
    }
    private void ChangeNextState()
    {
        if(InputManager.Instance.MoveInput.sqrMagnitude > 0.01f)
        {
            stateMachine.ChangeState(PlayerStateType.Move);
        }
        else
        {
            stateMachine.ChangeState(PlayerStateType.Idle);
        }
    }
}
