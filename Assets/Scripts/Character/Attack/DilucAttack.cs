using UnityEngine;

public class DilucAttack : MonoBehaviour, ICharacterAttack
{
    private int attackIndex;
    private bool nextAttackInput;
    private bool isFinished;

    private CharacterRunTime character;
    private PlayerAnimationController playerAnimationController;
    [SerializeField] private AttackHitbox attackHitbox;

    public bool IsFinished => isFinished;

    public void Enter(
        CharacterRunTime characterRunTime,
        PlayerAnimationController playerAnimationController)
    {
        character = characterRunTime;
        this.playerAnimationController = playerAnimationController;

        attackIndex = 0;
        nextAttackInput = false;
        isFinished = false;

        playerAnimationController.SetAttackIndex(attackIndex);

        SetAttackData();

        playerAnimationController.PlayAttack();
    }

    public void AttackUpdate()
    {
        AnimatorStateInfo stateInfo =
            playerAnimationController.GetCurrentStateInfo();

        if (playerAnimationController.IsTransition())
        {
            return;
        }

        if (stateInfo.normalizedTime >= 1f)
        {
            if (nextAttackInput && attackIndex < 2)
            {
                attackIndex++;
                nextAttackInput = false;

                playerAnimationController.SetAttackIndex(
                    attackIndex
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
        if (attackIndex >= 2)
        {
            return;
        }

        nextAttackInput = true;
    }

    public void Exit()
    {
        DisableAttackHitbox();

        nextAttackInput = false;
    }

    private void SetAttackData()
    {
        float atk = character.ATK;

        float multiplier =
            character.Data.AttackMultipliers[attackIndex];

        attackHitbox.SetAttack(
            atk,
            multiplier,
            ElementType.Physical
        );
    }
    private void EnableAttackHitbox()
    {
        attackHitbox.gameObject.SetActive(true);
    }
    private void DisableAttackHitbox()
    {
        attackHitbox.gameObject.SetActive(false);
        attackHitbox.ClearHitbox();
    }
}