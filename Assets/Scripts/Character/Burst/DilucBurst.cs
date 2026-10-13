using UnityEngine;

public class DilucBurst : MonoBehaviour, ICharacterBurst
{
    [Header("Projectile")]
    [SerializeField] private AttackProjectile projectilePrefab;
    [SerializeField] private Transform spawnPoint;

    private bool isFinished;
    private bool enteredBurstAnimation;
    private CharacterRunTime character;
    private PlayerAnimationController playerAnimationController;

    public bool IsFinished => isFinished;

    public void Enter(
        CharacterRunTime characterRunTime,
        PlayerAnimationController playerAnimationController)
    {
        character = characterRunTime;
        this.playerAnimationController = playerAnimationController;

        isFinished = false;
        enteredBurstAnimation = false;

        playerAnimationController.PlayBurst();

    }

    public void BurstUpdate()
    {
        AnimatorStateInfo stateInfo =
            playerAnimationController.GetCurrentStateInfo();

        // 실제 Burst 애니메이션 진입 확인
        if (!enteredBurstAnimation)
        {
            if (stateInfo.IsTag("Burst"))
            {
                enteredBurstAnimation = true;
            }

            return;
        }

        // Burst에 진입한 이후 Burst 애니메이션 진행률만 검사
        if (!stateInfo.IsTag("Burst"))
        {
            return;
        }

        if (stateInfo.normalizedTime >= 0.95f)
        {
            isFinished = true;
        }
    }

    public void Exit()
    {

    }
    private void SpawnBurstProjectile()
    {
        float atk = character.Stat.ATK;
        float multiplier =
            character.Data.CharacterCombatData.GetBurstMultipliers(0);

        float damage = atk * multiplier;

        Transform characterTransform =
            CharacterManager.Instance.CharacterView.CurrentView.transform;

        Vector3 direction = characterTransform.forward;

        AttackProjectile attackProjectile =
            Instantiate(
                projectilePrefab,
                spawnPoint.position,
                Quaternion.LookRotation(direction)
            );

        attackProjectile.Initialize(
            damage,
            character.Data.ElementType,
            direction
        );
    }
}