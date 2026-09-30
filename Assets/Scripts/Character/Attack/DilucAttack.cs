using UnityEngine;

public class DilucAttack : MonoBehaviour, ICharacterAttack
{
    private int normalAttackIndex;
    private bool nextAttackInput;
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

        normalAttackIndex = 1;
        nextAttackInput = false;
        isFinished = false;

        attackHitbox = GetComponentInChildren<AttackHitbox>(true);

        attackHitbox.ClearHitbox();
        attackHitbox.gameObject.SetActive(true);

        playerAnimationController.SetNormalAttackIndex(normalAttackIndex);

        SetAttackData();

        playerAnimationController.PlayNormalAttack();

        Debug.Log("Diluc 평타 시작");
    }

    public void AttackUpdate()
    {
        AnimatorStateInfo stateInfo =
            playerAnimationController.GetCurrentStateInfo();

        if (!stateInfo.IsTag("NormalAttack"))
        {
            return;
        }

        if (playerAnimationController.IsTransition())
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

                playerAnimationController.SetNormalAttackIndex(
                    normalAttackIndex
                );

                SetAttackData();
            }
            else
            {
                isFinished = true;
            }
        }
    }

    public void OnAttackInput()
    {
        if (normalAttackIndex >= 3)
        {
            return;
        }

        nextAttackInput = true;
    }

    public void Exit()
    {
        attackHitbox.ClearHitbox();
        attackHitbox.gameObject.SetActive(false);

        nextAttackInput = false;

        Debug.Log("Diluc 평타 종료");
    }

    private void SetAttackData()
    {
        float atk = character.Stat.ATK;

        float multiplier =
            character.Data.CharacterCombatData
                .GetNormalAttackMultipliers(normalAttackIndex - 1);

        attackHitbox.SetAttack(
            atk,
            multiplier,
            ElementType.Physical
        );
    }
}