using UnityEngine;
using PixPlays.ElementalVFX;

public class BarbaraAttack : MonoBehaviour, ICharacterAttack
{
    [Header("Projectile VFX")]
    [SerializeField] private ProjectileVfx waterBulletPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float projectileRange = 10f;

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
    }

    private void SpawnNormalAttackProjectile()
    {
        float atk = character.Stat.ATK;

        float multiplier =
            character.Data.CharacterCombatData
                .GetNormalAttackMultipliers(normalAttackIndex - 1);

        float damage = atk * multiplier;

        Vector3 source = spawnPoint.position;

        Transform characterTransform =
            CharacterManager.Instance.CharacterView.CurrentView.transform;

        Vector3 direction = characterTransform.forward;

        Vector3 target =
            source + direction * 10f;

        ProjectileVfx waterBullet =
            Instantiate(waterBulletPrefab);

        waterBullet.SetAttack(
            damage,
            character.Data.ElementType
        );

        VfxData data =
            new VfxData(
                source,
                target,
                5f,
                0f
            );

        waterBullet.Play(data);
    }
}