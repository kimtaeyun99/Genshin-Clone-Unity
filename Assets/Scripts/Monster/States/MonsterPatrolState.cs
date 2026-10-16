using System;
using UnityEngine;
using UnityEngine.AI;

public class MonsterPatrolState : MonsterStateBase
{
    private const int InitialDetectionBufferSize = 32;

    private Collider[] detectionBuffer = new Collider[InitialDetectionBufferSize];

    public MonsterPatrolState(
        MonsterController monster,
        StateMachine<MonsterStateType> stateMachine)
        : base(monster, stateMachine)
    {
    }

    public override void Enter()
    {
        monster.Agent.isStopped = false;
        monster.Agent.stoppingDistance = 0.2f;

        monster.MonsterAnimationController.PlayPatrol();
        SetRandomDestination();
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
            return;

        if (monster.Agent.remainingDistance <= monster.Agent.stoppingDistance)
            stateMachine.ChangeState(MonsterStateType.Idle);
    }

    public override void Exit()
    {
        monster.Agent.ResetPath();
    }

    private void SetRandomDestination()
    {
        Vector2 randomPoint = UnityEngine.Random.insideUnitCircle * monster.PatrolRadius;
        Vector3 targetPosition = monster.SpawnPosition +
                                 new Vector3(randomPoint.x, 0f, randomPoint.y);

        if (NavMesh.SamplePosition(
                targetPosition,
                out NavMeshHit hit,
                monster.PatrolRadius,
                NavMesh.AllAreas))
        {
            monster.Agent.SetDestination(hit.position);
        }
    }

    private void DetectPlayer()
    {
        int hitCount;

        while (true)
        {
            hitCount = Physics.OverlapSphereNonAlloc(
                monster.transform.position,
                monster.DetectionRadius,
                detectionBuffer
            );

            if (hitCount < detectionBuffer.Length)
                break;

            Array.Resize(ref detectionBuffer, detectionBuffer.Length * 2);
        }

        for (int i = 0; i < hitCount; i++)
        {
            Collider col = detectionBuffer[i];
            if (col != null && col.CompareTag("Player"))
            {
                monster.SetTarget(col.transform);
                return;
            }
        }
    }
}
