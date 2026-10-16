using UnityEngine;

public class DilucSkill : MonoBehaviour, ICharacterSkill
{
    [SerializeField] private GameObject fireVFX;

    private int skillIndex;
    private bool nextSkillInput;
    private bool isFinished;

    private CharacterRunTime character;
    private PlayerAnimationController playerAnimationController;
    [SerializeField] private AttackHitbox attackHitbox;

    public bool IsFinished => isFinished;
    private bool gainedEnergyThisHit;

    public void Enter(
        CharacterRunTime characterRunTime,
        PlayerAnimationController playerAnimationController)
    {
        character = characterRunTime;
        this.playerAnimationController = playerAnimationController;
        fireVFX.gameObject.SetActive(true);
        skillIndex = 0;
        nextSkillInput = false;
        isFinished = false;
        attackHitbox.OnHit += HandleSkillHit;

        playerAnimationController.SetSkillIndex(skillIndex);

        SetSkillData();

        playerAnimationController.PlaySkill();
    }

    public void SkillUpdate()
    {
        if (playerAnimationController.IsTransition())
            return;

        AnimatorStateInfo stateInfo =
            playerAnimationController.GetCurrentStateInfo();

        string skillName = skillIndex switch
        {
            0 => "Skill1",
            1 => "Skill2",
            2 => "Skill3",
            _ => ""
        };

        if (!stateInfo.IsName(skillName))
            return;

        if (stateInfo.normalizedTime < 1f)
            return;

        if (nextSkillInput && skillIndex < 2)
        {
            skillIndex++;
            nextSkillInput = false;

            playerAnimationController.SetSkillIndex(skillIndex);
            SetSkillData();
        }
        else
        {
            isFinished = true;
        }
    }

    public void OnSkillInput()
    {
        if (skillIndex >= 2)
        {
            return;
        }

        nextSkillInput = true;
    }

    public void Exit()
    {
        attackHitbox.OnHit -= HandleSkillHit;
        nextSkillInput = false;
        DisableSkillHitbox();
        fireVFX.gameObject.SetActive(false);
    }

    private void SetSkillData()
    {
        float atk = character.ATK;

        float multiplier =
            character.Data.SkillMultipliers[skillIndex];

        attackHitbox.SetAttack(
            atk,
            multiplier,
            character.Data.ElementType
        );
    }
    private void EnableSkillHitbox()
    {
        gainedEnergyThisHit = false;
        attackHitbox.gameObject.SetActive(true);
    }
    private void DisableSkillHitbox()
    {
        attackHitbox.gameObject.SetActive(false);
        attackHitbox.ClearHitbox();
    }
    private void HandleSkillHit()
    {
        if (gainedEnergyThisHit)
        {
            return;
        }
        character.AddEnergy(17);
        gainedEnergyThisHit = true;
    }
}