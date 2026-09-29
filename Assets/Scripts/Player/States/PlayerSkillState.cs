using UnityEngine;

public class PlayerSkillState : PlayerStateBase
{
    private int skillIndex;
    private AnimatorStateInfo stateInfo;
    private bool nextSkillInput;
    private AttackHitbox attackHitbox;
    public PlayerSkillState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {

    }
    public override void Enter()
    {
        attackHitbox = player.CharacterManager.CurrentAttackHitbox;
        attackHitbox.gameObject.SetActive(true);
        skillIndex = 1;
        nextSkillInput = false;

        player.Animation.SetSkillIndex(skillIndex);

        SetSkillData();

        player.Animation.PlaySkill();

        InputManager.Instance.OnSkill += OnSkill;

        Debug.Log("Skill 진입");
    }
    public override void Update()
    {
        stateInfo = player.Animation.GetCurrentStateInfo();

        if (!stateInfo.IsTag("Skill"))
        {
            return;
        }

        if (player.Animation.IsTransition())
        {
            return;
        }

        if (stateInfo.normalizedTime >= 1f)
        {
            if (nextSkillInput && skillIndex < 3)
            {
                skillIndex++;
                nextSkillInput = false;
                attackHitbox.ClearHitbox();

                player.Animation.SetSkillIndex(skillIndex);

                SetSkillData();

                Debug.Log($"{skillIndex}타 시작");
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

        InputManager.Instance.OnSkill -= OnSkill;

        Debug.Log("Skill 퇴장");
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
    private void OnSkill()
    {
        if (skillIndex >= 3)
        {
            return;
        }

        nextSkillInput = true;

        Debug.Log("다음 스킬 입력 저장");
    }
    private void SetSkillData()
    {
        CharacterRunTime character = player.CharacterManager.CurrentCharacter;

        float atk = character.Stat.ATK;

        float multiplier = character.Data.CharacterCombatData.GetSkillMultipliers(skillIndex -1);

        attackHitbox.SetAttack(atk, multiplier, character.Data.ElementType);
    }
}