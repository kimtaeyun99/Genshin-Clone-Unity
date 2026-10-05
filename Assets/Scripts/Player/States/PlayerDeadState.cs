using UnityEngine;

public class PlayerDeadState : PlayerStateBase
{
    private bool finished;

    public PlayerDeadState(PlayerController player, PlayerStateMachine stateMachine)
        : base(player, stateMachine) { }

    public override void Enter()
    {
        finished = false;
        player.Health.ResetElement();
        player.Animation.PlayDead();
    }

    public override void Update()
    {
        if (finished) return;

        if (!IsDeadAnimationEnd()) return;

        finished = true;
        OnDeadAnimationEnd();
    }

    private bool IsDeadAnimationEnd()
    {
        if (player.Animation.IsTransition()) return false;

        AnimatorStateInfo info = player.Animation.GetCurrentStateInfo();

        return info.normalizedTime >= 1f;
    }

    private void OnDeadAnimationEnd()
    {
        if (CharacterParty.Instance.SelectNextAlive())
        {
            stateMachine.ChangeState(PlayerStateType.Idle);
        }
        else
        {
            //파티원전멸처리
        }
    }
}