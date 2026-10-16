using UnityEngine;

public class DilucBurst : MonoBehaviour, ICharacterBurst
{
    [Header("Projectile")]
    [SerializeField] private AttackProjectile projectilePrefab;
    [SerializeField] private Transform spawnPoint;

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

        isFinished = false;

        playerAnimationController.PlayBurst();

    }

    public void BurstUpdate()
    {
        AnimatorStateInfo stateInfo =
            playerAnimationController.GetCurrentStateInfo();

        if (stateInfo.IsName("Burst") &&
            stateInfo.normalizedTime >= 0.9f)
        {
            isFinished = true;
        }
    }

    public void Exit()
    {

    }
    private void SpawnBurstProjectile()
    {
        float atk = character.ATK;
        float multiplier =
            character.Data.BurstMultipliers[0];

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