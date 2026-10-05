using UnityEngine;

public class MonsterChaseState : MonsterStateBase
{
    public MonsterChaseState(
        MonsterController monster,
        MonsterStateMachine stateMachine)
        : base(monster, stateMachine)
    {
    }

    public override void Enter()
    {
        monster.Animation.PlayChase();

        if (monster.Target != null)
        {
            monster.Agent.SetDestination(monster.Target.position);
        }

        Debug.Log("Monster Chase 진입");
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

        // 추적 포기
        if (distance > monster.ChaseRange)
        {
            monster.ClearTarget();
            stateMachine.ChangeState(MonsterStateType.Idle);
            return;
        }

        // 공격 범위 진입
        if (distance <= monster.AttackRange)
        {
            stateMachine.ChangeState(MonsterStateType.Attack);
            return;
        }

        // 플레이어 추적
        monster.Agent.SetDestination(monster.Target.position);
    }

    public override void Exit()
    {
        Debug.Log("Monster Chase 종료");
    }
}