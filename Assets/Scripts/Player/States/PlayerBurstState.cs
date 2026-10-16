using UnityEngine;

public class PlayerBurstState : PlayerStateBase
{
    private ICharacterBurst currentBurst;
    private CharacterRunTime character;
    private bool burstStarted;
    public PlayerBurstState(
        PlayerController player,
        StateMachine<PlayerStateType> stateMachine)
        : base(player, stateMachine)
    {
    }

    public override void Enter()
    {
        burstStarted = false;

        character = CharacterManager.Instance.CurrentCharacter;
        currentBurst = CharacterManager.Instance.CurrentBurst;

        if (character == null || currentBurst == null)
        {
            ChangeNextState();
            return;
        }

        if (!character.ConsumeBurstEnergy())
        {
            ChangeNextState();
            return;
        }

        burstStarted = true;
        player.StartLockOn();
        currentBurst.Enter(
            character,
            player.PlayerAnimationController
        );
    }

    public override void Update()
    {
        if (currentBurst == null)
            return;

        player.UpdateLockOn();

        if (player.PlayerAnimationController.IsTransition())
            return;

        currentBurst.BurstUpdate();

        if (currentBurst.IsFinished)
            ChangeNextState();
    }

    public override void Exit()
    {
        player.EndLockOn();
        if (burstStarted)
        {
            currentBurst?.Exit();

            character.StartBurstCoolDown(
                character.Data.BurstCoolDown
            );
        }

        burstStarted = false;
        currentBurst = null;
        character = null;
    }
}