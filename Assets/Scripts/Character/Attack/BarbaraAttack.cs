using UnityEngine;

public class BarbaraAttack : MonoBehaviour, ICharacterAttack
{
    [Header("Projectile")]
    [SerializeField] private AttackProjectile projectilePrefab;
    [SerializeField] private Transform spawnPoint;

    private int normalAttackIndex;
    private bool nextAttackInput;
    private bool isFinished;

    private CharacterRunTime character;
    private PlayerAnimationController playerAnimationController;

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

        playerAnimationController.SetNormalAttackIndex(normalAttackIndex);

        playerAnimationController.PlayNormalAttack();

        Debug.Log("Barbara 평타 시작");
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
            if (nextAttackInput && normalAttackIndex < 4)
            {
                normalAttackIndex++;
                nextAttackInput = false;

                playerAnimationController.SetNormalAttackIndex(
                    normalAttackIndex
                );
            }
            else
            {
                isFinished = true;
            }
        }
    }

    public void OnAttackInput()
    {
        if (normalAttackIndex >= 4)
        {
            return;
        }

        nextAttackInput = true;
    }

    public void Exit()
    {
        nextAttackInput = false;

        Debug.Log("Barbara 평타 종료");
    }

    private void SpawnProjectile()
    {
        float atk = character.Stat.ATK;

        float multiplier =
            character.Data.CharacterCombatData
                .GetNormalAttackMultipliers(normalAttackIndex - 1);

        float damage = atk * multiplier;

        AttackProjectile projectile = Instantiate(
            projectilePrefab,
            spawnPoint.position,
            spawnPoint.rotation
        );

        projectile.Initialize(
            damage,
            character.Data.ElementType,
            transform.forward
        );
    }
}