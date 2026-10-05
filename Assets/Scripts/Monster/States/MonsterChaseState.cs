using UnityEngine;

public class MonsterChaseState : MonsterStateBase
{
    private float attackTimer;
    public MonsterChaseState(
        MonsterController monster,
        MonsterStateMachine stateMachine)
        : base(monster, stateMachine)
    {
    }

    public override void Enter()
    {
        monster.Animation.PlayChase();

        attackTimer = 0f;

        monster.Agent.isStopped = false;

        if (monster.Target != null)
        {
            monster.Agent.SetDestination(monster.Target.position);
        }

    }

    public override void Update()
    {
        attackTimer += Time.deltaTime;

        if (monster.Target == null)
        {
            stateMachine.ChangeState(MonsterStateType.Patrol);
            return;
        }
        float distance = Vector3.Distance(
            monster.transform.position,
            monster.Target.position
        );
        if (distance > monster.ChaseRange)
        {
            monster.ClearTarget();
            stateMachine.ChangeState(MonsterStateType.Patrol);
            return;
        }
        if (distance <= monster.AttackRange && attackTimer >= monster.AttackDelay)
        {
            Debug.Log("Chase → Attack 전환");
            stateMachine.ChangeState(MonsterStateType.Attack);
            return;
        }
        monster.Agent.SetDestination(monster.Target.position);
    }

    public override void Exit()
    {
        Debug.Log("Monster Chase 종료");
    }
}