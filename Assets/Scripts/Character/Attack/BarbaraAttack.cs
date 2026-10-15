using UnityEngine;
using PixPlays.ElementalVFX;

public class BarbaraAttack : MonoBehaviour, ICharacterAttack
{
    [Header("Projectile VFX")]
    [SerializeField] private ProjectileVfx waterBulletPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float projectileRange = 10f;

    private int attackIndex;
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

        attackIndex = 0;
        nextAttackInput = false;
        isFinished = false;

        playerAnimationController.SetAttackIndex(attackIndex);
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
            if (nextAttackInput && attackIndex < 3)
            {
                attackIndex++;
                nextAttackInput = false;

                playerAnimationController.SetAttackIndex(
                    attackIndex
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
        if (attackIndex >= 3)
        {
            return;
        }

        nextAttackInput = true;
    }

    public void Exit()
    {
        nextAttackInput = false;
    }

    private void SpawnAttackProjectile()
    {
        float atk = character.ATK;

        float multiplier =
            character.Data.AttackMultipliers[attackIndex];

        float damage = atk * multiplier;

        Vector3 source = spawnPoint.position;

        Transform characterTransform =
            CharacterManager.Instance.CharacterView.CurrentView.transform;

        Vector3 direction = characterTransform.forward;

        Vector3 target =
            source + direction * projectileRange;

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