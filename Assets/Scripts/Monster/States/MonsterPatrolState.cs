using UnityEngine;
using UnityEngine.AI;

public class MonsterPatrolState : MonsterStateBase
{
    public MonsterPatrolState(MonsterController monster,MonsterStateMachine stateMachine) : base(monster, stateMachine)
    {
    }

    public override void Enter()
    {
        SetRandomDestination();
        Debug.Log("Patrol ¡¯¿‘");
    }

    public override void Update()
    {
        DetectPlayer();

        if (monster.Target != null)
        {
            stateMachine.ChangeState(MonsterStateType.Chase);
            return;
        }

        if (monster.Agent.pathPending)
        {
            return;
        }

        if (monster.Agent.remainingDistance <= monster.Agent.stoppingDistance)
        {
            stateMachine.ChangeState(MonsterStateType.Idle);
        }
    }

    public override void Exit()
    {
        monster.Agent.ResetPath();
        Debug.Log("Patrol ≈¿Â");
    }

    private void SetRandomDestination()
    {
        Vector2 randomPoint = Random.insideUnitCircle * monster.PatrolRadius;

        Vector3 targetPosition = monster.SpawnPosition + new Vector3(randomPoint.x, 0f, randomPoint.y);

        if (NavMesh.SamplePosition( targetPosition, out NavMeshHit hit, monster.PatrolRadius, NavMesh.AllAreas))
        {
            monster.Agent.SetDestination(hit.position);
        }
    }
    private void DetectPlayer()
    {
        Collider[] colliders = Physics.OverlapSphere(
            monster.transform.position,
            monster.DetectionRadius
        );

        foreach (Collider col in colliders)
        {
            if (col.CompareTag("Player"))
            {
                monster.SetTarget(col.transform);
                return;
            }
        }
    }
}