using UnityEngine;

public class PlayerBurstState : PlayerStateBase
{
    private AnimatorStateInfo stateInfo;
    public PlayerBurstState(PlayerController player, PlayerStateMachine stateMachine) : base(player,stateMachine)
    {

    }
    public override void Enter()
    {
        player.Animation.PlayBurst();

        Debug.Log("Burst ÁøÀÔ");
    }
    public override void Update()
    {
        stateInfo = player.Animation.GetCurrentStateInfo();

        if (!stateInfo.IsTag("Burst"))
        {
            return;
        }

        if (stateInfo.normalizedTime >= 0.95f)
        {
            ChangeNextState();
        }
    }
    public override void Exit()
    {
        Debug.Log("Burst ÅðÀå");
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
