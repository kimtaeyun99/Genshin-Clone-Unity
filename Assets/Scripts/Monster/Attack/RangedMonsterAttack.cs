using UnityEngine;

public class RangedMonsterAttack : MonoBehaviour, IMonsterAttack
{
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

        // Attack1 : 화살 준비
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

        AnimatorStateInfo stateInfo =
            monster.Animation.GetCurrentStateInfo();

        // Attack1
        if (attackStep == 0)
        {
            if (!stateInfo.IsName("Attack1"))
            {
                return;
            }

            if (stateInfo.normalizedTime >= 1f)
            {
                attackStep = 1;

                LookTarget();

                // Attack2 : 화살 발사
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

        Vector3 direction =
            monster.Target.position - monster.transform.position;

        direction.y = 0f;

        if (direction.sqrMagnitude <= 0.01f)
        {
            return;
        }

        monster.transform.rotation =
            Quaternion.LookRotation(direction);
    }
}