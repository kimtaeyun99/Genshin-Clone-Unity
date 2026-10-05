using UnityEngine;

public class RangedMonsterAttack : MonoBehaviour, IMonsterAttack
{
    [SerializeField] private AttackProjectile arrowPrefab;
    [SerializeField] private Transform spawnPoint;

    private MonsterController monster;

    private int attackStep;
    private bool isFinished;

    public bool IsFinished => isFinished;

    public void Enter(MonsterController monster)
    {
        this.monster = monster;

        attackStep = 0;
        isFinished = false;

        LookTarget();
        monster.Animation.PlayAttack(0);
    }

    public void AttackUpdate()
    {
        if (isFinished)
        {
            return;
        }

        if (monster.Animation.IsTransition())
        {
            return;
        }

        AnimatorStateInfo stateInfo = monster.Animation.GetCurrentStateInfo();

        if (attackStep == 0)
        {
            if (!stateInfo.IsName("Attack1"))
            {
                return;
            }

            if (stateInfo.normalizedTime >= 1f)
            {
                attackStep = 1;

                AttackProjectile arrow = Instantiate(arrowPrefab, spawnPoint.position, spawnPoint.rotation);
                arrow.Initialize(monster.Attack, monster.ElementType);

                monster.Animation.PlayAttack(1);
            }

            return;
        }

        // Attack2
        if (attackStep == 1)
        {
            if (!stateInfo.IsName("Attack2"))
            {
                return;
            }

            if (stateInfo.normalizedTime >= 1f)
            {
                attackStep = 2;
                isFinished = true;
            }
        }
    }

    public void Exit()
    {
        monster = null;

        attackStep = 0;
        isFinished = false;
    }

    private void LookTarget()
    {
        if (monster.Target == null)
        {
            return;
        }

        Vector3 direction = monster.Target.position - monster.transform.position;

        direction.y = 0f;

        if (direction.sqrMagnitude <= 0.01f)
        {
            return;
        }

        Quaternion lookRotation = Quaternion.LookRotation(direction);
        Quaternion offsetRotation = Quaternion.Euler(0f, -230f, 0f);

        monster.transform.rotation = lookRotation * offsetRotation;
    }
}