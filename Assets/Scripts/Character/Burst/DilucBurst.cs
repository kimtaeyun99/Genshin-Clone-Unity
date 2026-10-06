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

        SpawnProjectile();

        Debug.Log("Diluc Burst 시작");
    }

    public void BurstUpdate()
    {
        AnimatorStateInfo stateInfo =
            playerAnimationController.GetCurrentStateInfo();

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
        Debug.Log("Diluc Burst 종료");
    }
    private void SpawnProjectile()
    {
        float atk = character.Stat.ATK;
        float multiplier = character.Data.CharacterCombatData.GetBurstMultipliers(0);

        float damage = atk * multiplier;

        AttackProjectile attackProjectile = Instantiate(projectilePrefab, spawnPoint.position, spawnPoint.rotation);

        attackProjectile.Initialize(damage, character.Data.ElementType,transform.forward);
    }
}