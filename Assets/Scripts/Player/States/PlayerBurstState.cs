using UnityEngine;

public class PlayerBurstState : PlayerStateBase
{
    private ICharacterBurst currentBurst;
    private CharacterRunTime character;

    public PlayerBurstState(
        PlayerController player,
        PlayerStateMachine stateMachine)
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
            player.Animation
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