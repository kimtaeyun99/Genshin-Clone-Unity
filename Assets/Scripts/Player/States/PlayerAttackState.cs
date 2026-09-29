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

        Debug.Log($"대시 해제 확인 : {player.Movement.IsDashMode}");
        player.Animation.SetNormalAttackIndex(normalAttackIndex);

        SetAttackData();

        player.Animation.PlayNormalAttack();
       

        InputManager.Instance.OnAttack += OnAttack;

        Debug.Log("Attack 진입");
        Debug.Log($"{normalAttackIndex}타 시작");
        Debug.Log($"공격력 : {player.CharacterManager.CurrentCharacter.Stat.ATK} ");
        Debug.Log($"배율 : {player.CharacterManager.CurrentCharacter.Data.CharacterCombatData.GetNormalAttackMultipliers(normalAttackIndex - 1)}");
        Debug.Log($"데미지 : {player.CharacterManager.CurrentAttackHitbox.Damage}");
        Debug.Log($"속성 : {player.CharacterManager.CurrentAttackHitbox.Element}");
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

                player.Animation.SetNormalAttackIndex(normalAttackIndex);

                SetAttackData();

                Debug.Log($"{normalAttackIndex}타 시작");
                Debug.Log($"공격력 : {player.CharacterManager.CurrentCharacter.Stat.ATK} ");
                Debug.Log($"배율 : {player.CharacterManager.CurrentCharacter.Data.CharacterCombatData.GetNormalAttackMultipliers(normalAttackIndex -1)}");
                Debug.Log($"데미지 : {player.CharacterManager.CurrentAttackHitbox.Damage}");
                Debug.Log($"속성 : {player.CharacterManager.CurrentAttackHitbox.Element}");
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

        Debug.Log("Attack 퇴장");
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

        Debug.Log("다음 평타 입력 저장");
    }
    private void SetAttackData()
    {
        CharacterRunTime character = player.CharacterManager.CurrentCharacter;

        float atk = character.Stat.ATK;

        float multiplier = character.Data.CharacterCombatData.GetNormalAttackMultipliers(normalAttackIndex - 1);

        player.CharacterManager.CurrentAttackHitbox.SetAttack(atk, multiplier, ElementType.Physical);
    }
}