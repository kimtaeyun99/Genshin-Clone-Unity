using UnityEngine;

public class PlayerAttackState : PlayerStateBase
{
    private ICharacterAttack currentAttack;

    public PlayerAttackState(
        PlayerController player,
        StateMachine<PlayerStateType> stateMachine)
        : base(player, stateMachine)
    {
    }

    public override void Enter()
    {
        currentAttack = CharacterManager.Instance.CurrentAttack;

        if (currentAttack == null)
        {
            ChangeNextState();
            return;
        }
        player.StartLockOn();
        currentAttack.Enter(
            CharacterManager.Instance.CurrentCharacter,
            player.PlayerAnimationController
        );

        InputManager.Instance.OnAttack += OnAttack;

    }

    public override void Update()
    {
        if (currentAttack == null)
            return;

        player.UpdateLockOn();

        currentAttack.AttackUpdate();

        if (currentAttack.IsFinished)
            ChangeNextState();
    }

    public override void Exit()
    {
        player.EndLockOn();
        InputManager.Instance.OnAttack -= OnAttack;

        currentAttack?.Exit();

        currentAttack = null;

    }

    private void OnAttack()
    {
        currentAttack?.OnAttackInput();
    }
}