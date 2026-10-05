using UnityEngine;

public class MonsterAttackState : MonsterStateBase
{
    private bool attackStarted;

    public MonsterAttackState(
        MonsterController monster,
        MonsterStateMachine stateMachine)
        : base(monster, stateMachine)
    {
    }

    public override void Enter()
    {
        monster.Agent.ResetPath();

        attackStarted = false;

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

        Debug.Log("Monster Attack 진입");
    }

    public override void Update()
    {
        if (monster.Target == null)
        {
            stateMachine.ChangeState(MonsterStateType.Idle);
            return;
        }

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
            stateMachine.ChangeState(MonsterStateType.Chase);
        }
    }

    public override void Exit()
    {
        monster.Hitbox.ClearHitbox();
        monster.Hitbox.gameObject.SetActive(false);

        Debug.Log("Monster Attack 종료");
    }

    private void LookTarget()
    {
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