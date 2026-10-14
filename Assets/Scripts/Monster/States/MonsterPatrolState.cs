using System;
using UnityEngine;
using UnityEngine.AI;

public class MonsterPatrolState : MonsterStateBase
{
    private const int InitialDetectionBufferSize = 32;

    // 몬스터별 PatrolState에서 한 번 생성해 재사용한다.
    private Collider[] detectionBuffer = new Collider[InitialDetectionBufferSize];

    public MonsterPatrolState(
        MonsterController monster,
        StateMachine<MonsterStateType> stateMachine)
        : base(monster, stateMachine)
    {
    }

    public override void Enter()
    {
        monster.Animation.PlayPatrol();
        SetRandomDestination();
    }

    public override void Update()
    {
        // 기존처럼 매 프레임 플레이어 탐지: 탐지 반응 주기 변경 없음.
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

        // NonAlloc 결과가 버퍼를 꽉 채운 경우 잘렸을 가능성이 있다.
        // 플레이어를 놓치지 않도록 버퍼를 늘려 즉시 다시 탐지한다.
        // 확장 시에만 배열 할당이 발생하고 이후에는 재사용된다.
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
