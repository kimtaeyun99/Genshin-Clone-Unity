using UnityEngine;

public class PlayerAttackState : PlayerStateBase
{
    private int normalAttackIndex;
    private AnimatorStateInfo stateInfo;
    private bool nextAttackInput;
    private AttackHitbox attackHitbox;
    public PlayerAttackState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {

    }
    public override void Enter()
    {
        attackHitbox = player.CharacterManager.CurrentAttackHitbox;
        attackHitbox.gameObject.SetActive(true);

        normalAttackIndex = 1;
        nextAttackInput = false;
        SetAttackData();
        player.Animation.SetNormalAttackIndex(normalAttackIndex);

        player.Animation.PlayNormalAttack();
       

        InputManager.Instance.OnAttack += OnAttack;
    }
    public override void Update()
    {
        stateInfo = player.Animation.GetCurrentStateInfo();

        if (!stateInfo.IsTag("NormalAttack"))
        {
            return;
        }

        if (player.Animation.IsTransition())
        {
            return;
        }

        if (stateInfo.normalizedTime >= 1f)
        {
            if (nextAttackInput && normalAttackIndex < 3)
            {
                normalAttackIndex++;
                nextAttackInput = false;
                attackHitbox.ClearHitbox();
                SetAttackData();
                player.Animation.SetNormalAttackIndex(normalAttackIndex);
            }
            else
            {
                ChangeNextState();
            }
        }
    }
    public override void Exit()
    {
        attackHitbox.ClearHitbox();
        attackHitbox.gameObject.SetActive(false);

        InputManager.Instance.OnAttack -= OnAttack;
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
    private void OnAttack()
    {
        if (normalAttackIndex >= 3)
        {
            return;
        }

        nextAttackInput = true;
    }
    private void SetAttackData()
    {
        CharacterRunTime character = player.CharacterManager.CurrentCharacter;

        float atk = character.Stat.ATK;

        float multiplier = character.Data.CharacterCombatData.GetNormalAttackMultipliers(normalAttackIndex - 1);

        player.CharacterManager.CurrentAttackHitbox.SetAttack(atk, multiplier, ElementType.Physical);
    }
}