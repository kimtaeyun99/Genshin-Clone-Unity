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
                character.Data.BurstCoolDown
            );
        }
        currentBurst = null;
        character = null;
    }
}