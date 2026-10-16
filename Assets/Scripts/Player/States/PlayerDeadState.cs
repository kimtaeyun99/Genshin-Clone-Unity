using UnityEngine;

public class PlayerDeadState : PlayerStateBase
{
    private bool finished;

    public PlayerDeadState(
        PlayerController player,
        StateMachine<PlayerStateType> stateMachine)
        : base(player, stateMachine)
    {
    }

    public override void Enter()
    {
        finished = false;

        player.Health.ResetElement();
        player.PlayerAnimationController.PlayDead();
    }

    public override void Update()
    {
        if (finished)
            return;

        if (!IsDeadAnimationEnd())
            return;

        finished = true;
        OnDeadAnimationEnd();
    }

    private bool IsDeadAnimationEnd()
    {
        if (player.PlayerAnimationController.IsTransition())
            return false;

        AnimatorStateInfo info =
            player.PlayerAnimationController.GetCurrentStateInfo();

        if (!info.IsName("Dead"))
            return false;

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
            GameOverManager.Instance.ShowGameOver();
        }
    }
}