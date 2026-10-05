using UnityEngine;

public class MonsterAttackState : MonsterStateBase
{
    public MonsterAttackState(
        MonsterController monster,
        MonsterStateMachine stateMachine)
        : base(monster, stateMachine)
    {
    }

    public override void Enter()
    {
        monster.Agent.ResetPath();

        monster.Animation.PlayAttack();

        Debug.Log("Monster Attack 진입");
    }

    public override void Update()
    {
        if (monster.Target == null)
        {
            stateMachine.ChangeState(MonsterStateType.Idle);
            return;
        }

        float distance = Vector3.Distance(
            monster.transform.position,
            monster.Target.position
        );

        // 공격 범위에서 벗어나면 다시 추적
        if (distance > monster.AttackRange)
        {
            stateMachine.ChangeState(MonsterStateType.Chase);
            return;
        }

        LookTarget();
    }

    public override void Exit()
    {
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