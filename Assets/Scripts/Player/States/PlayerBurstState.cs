using UnityEngine;

public class PlayerBurstState : PlayerStateBase
{
    private ICharacterBurst currentBurst;

    public PlayerBurstState(
        PlayerController player,
        PlayerStateMachine stateMachine)
        : base(player, stateMachine)
    {
    }

    public override void Enter()
    {
        currentBurst = player.CharacterManager.CurrentBurst;

        if (currentBurst == null)
        {
            ChangeNextState();
            return;
        }

        currentBurst.Enter(
            player.CharacterManager.CurrentCharacter,
            player.Animation
        );

        Debug.Log("Burst ÁøÀÔ");
    }

    public override void Update()
    {
        if (currentBurst == null)
        {
            return;
        }

        currentBurst.BurstUpdate();

        if (currentBurst.IsFinished)
        {
            ChangeNextState();
        }
    }

    public override void Exit()
    {
        currentBurst?.Exit();

        currentBurst = null;

        Debug.Log("Burst ÅðÀå");
    }

    private void ChangeNextState()
    {
        if (InputManager.Instance.MoveInput.sqrMagnitude > 0.01f)
        {
            stateMachine.ChangeState(PlayerStateType.Move);
        }
        else
        {
            stateMachine.ChangeState(PlayerStateType.Idle);
        }
    }
}