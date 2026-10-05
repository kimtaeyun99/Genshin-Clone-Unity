using UnityEngine;

public class MeleeMonsterAttack : MonoBehaviour, IMonsterAttack
{
    private MonsterController monster;
    private bool attackStarted;
    private bool isFinished;

    public bool IsFinished => isFinished;

    public void Enter(MonsterController monster)
    {
        this.monster = monster;

        attackStarted = false;
        isFinished = false;

        LookTarget();

        monster.Hitbox.ClearHitbox();
        monster.Hitbox.gameObject.SetActive(true);

        monster.Hitbox.SetAttack(
            monster.Attack,
            monster.AttackMultiplier,
            monster.ElementType
        );

        int attackIndex = Random.Range(0, 2);

        monster.Animation.PlayAttack(attackIndex);
    }

    public void AttackUpdate()
    {
        if (monster.Animation.IsTransition())
        {
            return;
        }

        if (!attackStarted)
        {
            attackStarted = true;
            return;
        }

        AnimatorStateInfo stateInfo =
            monster.Animation.GetCurrentStateInfo();

        if (stateInfo.normalizedTime >= 1f)
        {
            isFinished = true;
        }
    }

    public void Exit()
    {
        monster.Hitbox.ClearHitbox();
        monster.Hitbox.gameObject.SetActive(false);
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