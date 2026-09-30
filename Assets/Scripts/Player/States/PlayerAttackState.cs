using UnityEngine;

public class PlayerAttackState : PlayerStateBase
{
    private ICharacterAttack currentAttack;

    public PlayerAttackState(
        PlayerController player,
        PlayerStateMachine stateMachine)
        : base(player, stateMachine)
    {
    }

    public override void Enter()
    {
        currentAttack = player.CharacterManager.CurrentAttack;

        if (currentAttack == null)
        {
            ChangeNextState();
            return;
        }

        currentAttack.Enter(
            player.CharacterManager.CurrentCharacter,
            player.Animation
        );

        InputManager.Instance.OnAttack += OnAttack;

        Debug.Log("Attack ÁøÀÔ");
    }

    public override void Update()
    {
        if (currentAttack == null)
        {
            return;
        }

        currentAttack.AttackUpdate();

        if (currentAttack.IsFinished)
        {
            ChangeNextState();
        }
    }

    public override void Exit()
    {
        InputManager.Instance.OnAttack -= OnAttack;

        currentAttack?.Exit();

        currentAttack = null;

        Debug.Log("Attack ÅðÀå");
    }

    private void OnAttack()
    {
        currentAttack?.OnAttackInput();
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