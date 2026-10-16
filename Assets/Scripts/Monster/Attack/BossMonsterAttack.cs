using UnityEngine;

public class BossMonsterAttack : MonoBehaviour, IMonsterAttack
{
    [SerializeField] private AttackHitbox longHitbox;
    [SerializeField] private AttackHitbox shortHitbox;
    [SerializeField] private float jumpAttackMultiplier = 3f;
    private MonsterController monster;

    private bool attackStarted;
    private bool isFinished;

    public bool IsFinished => isFinished;

    public void Enter(MonsterController monster)
    {
        this.monster = monster;

        attackStarted = false;
        isFinished = false;

        longHitbox.SetAttack(
            monster.Runtime.ATK,
            monster.AttackMultiplier * jumpAttackMultiplier,
            monster.Data.ElementType
        );

        shortHitbox.SetAttack(
            monster.Runtime.ATK,
            monster.AttackMultiplier,
            monster.Data.ElementType
        );

        DisableHitboxes();

        LookTarget();

        int attackIndex = Random.Range(0, 3);

        monster.MonsterAnimationController.PlayAttack(attackIndex);
    }

    public void AttackUpdate()
    {
        if (monster.MonsterAnimationController.IsTransition())
            return;

        if (!attackStarted)
        {
            attackStarted = true;
            return;
        }

        AnimatorStateInfo stateInfo =
            monster.MonsterAnimationController.GetCurrentStateInfo();

        if (stateInfo.normalizedTime >= 1f)
        {
            isFinished = true;
        }
    }

    public void Exit()
    {
        DisableHitboxes();
    }

    private void LookTarget()
    {
        if (monster.Target == null)
            return;

        Vector3 direction =
            monster.Target.position - monster.transform.position;

        direction.y = 0f;

        if (direction.sqrMagnitude <= 0.01f)
            return;

        monster.transform.rotation =
            Quaternion.LookRotation(direction);
    }

    private void EnableLongHitbox()
    {
        longHitbox.ClearHitbox();
        longHitbox.gameObject.SetActive(true);
    }

    private void DisableLongHitbox()
    {
        longHitbox.gameObject.SetActive(false);
        longHitbox.ClearHitbox();
    }

    private void EnableShortHitbox()
    {
        shortHitbox.ClearHitbox();
        shortHitbox.gameObject.SetActive(true);
    }

    private void DisableShortHitbox()
    {
        shortHitbox.gameObject.SetActive(false);
        shortHitbox.ClearHitbox();
    }

    private void DisableHitboxes()
    {
        DisableLongHitbox();
        DisableShortHitbox();
    }
}