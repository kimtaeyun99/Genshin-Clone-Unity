using UnityEngine;

public class PlayerBurstState : PlayerStateBase
{
    private ICharacterBurst currentBurst;
    private CharacterRunTime character;

    public PlayerBurstState(
        PlayerController player,
        StateMachine<PlayerStateType> stateMachine)
        : base(player, stateMachine)
    {
    }

    public override void Enter()
    {
        character = player.CharacterManager.CurrentCharacter;
        currentBurst = player.CharacterManager.CurrentBurst;

        if (character == null || currentBurst == null)
        {
            ChangeNextState();
            return;
        }
        character.ConsumeBurstEnergy();

        currentBurst.Enter(
            character,
            player.PlayerAnimationController
        );

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

        if (character != null)
        {
            character.StartBurstCoolDown(
                character.Data.BurstCooldown
            );
        }

        currentBurst = null;
        character = null;

    }
}