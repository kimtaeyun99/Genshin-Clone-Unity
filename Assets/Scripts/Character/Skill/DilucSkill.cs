using UnityEngine;

public class DilucSkill : MonoBehaviour, ICharacterSkill
{
    private int skillIndex;
    private bool nextSkillInput;
    private bool isFinished;

    private CharacterRunTime character;
    private PlayerAnimationController playerAnimationController;
    private AttackHitbox attackHitbox;

    public bool IsFinished => isFinished;

    public void Enter(
        CharacterRunTime characterRunTime,
        PlayerAnimationController playerAnimationController)
    {
        character = characterRunTime;
        this.playerAnimationController = playerAnimationController;

        skillIndex = 1;
        nextSkillInput = false;
        isFinished = false;

        attackHitbox = GetComponentInChildren<AttackHitbox>(true);

        attackHitbox.ClearHitbox();
        attackHitbox.gameObject.SetActive(true);
        playerAnimationController.SetSkillIndex(skillIndex);

        SetSkillData();

        playerAnimationController.PlaySkill();
    }

    public void SkillUpdate()
    {
        AnimatorStateInfo stateInfo = playerAnimationController.GetCurrentStateInfo();

        if (!stateInfo.IsTag("Skill"))
        {
            return;
        }

        if (playerAnimationController.IsTransition())
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

                playerAnimationController.SetSkillIndex(skillIndex);

                SetSkillData();
            }
            else
            {
                isFinished = true;
            }
        }
    }

    public void OnSkillInput()
    {
        if (skillIndex >= 3)
        {
            return;
        }

        nextSkillInput = true;
    }

    public void Exit()
    {
        nextSkillInput = false;
        attackHitbox.ClearHitbox();
        attackHitbox.gameObject.SetActive(false);
    }

    private void SetSkillData()
    {
        float atk = character.Stat.ATK;

        float multiplier =
            character.Data.CharacterCombatData
                .GetSkillMultipliers(skillIndex - 1);

        attackHitbox.SetAttack(
            atk,
            multiplier,
            character.Data.ElementType
        );
    }
}