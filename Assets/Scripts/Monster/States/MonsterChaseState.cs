using UnityEngine;

public class MonsterChaseState : MonsterStateBase
{
    // 5cm 이동만 허용 오차로 사용: 플레이어 이동 시 거의 즉시 목적지 갱신.
    // 매 프레임 경로 요청 대신 위치 변화/경로 소실 시에만 요청한다.
    private const float DestinationUpdateDistance = 0.05f;
    private const float DestinationUpdateDistanceSqr =
        DestinationUpdateDistance * DestinationUpdateDistance;

    private float attackTimer;
    private Vector3 lastRequestedDestination;
    private bool hasRequestedDestination;

    public MonsterChaseState(
        MonsterController monster,
        StateMachine<MonsterStateType> stateMachine)
        : base(monster, stateMachine)
    {
    }

    public override void Enter()
    {
        monster.Animation.PlayChase();
        attackTimer = 0f;
        hasRequestedDestination = false;
        monster.Agent.isStopped = false;

        if (monster.Target != null)
            UpdateDestination(monster.Target.position);
    }

    public override void Update()
    {
        attackTimer += Time.deltaTime;

        if (monster.Target == null)
        {
            stateMachine.ChangeState(MonsterStateType.Patrol);
            return;
        }

        Vector3 targetPosition = monster.Target.position;
        Vector3 toTarget = targetPosition - monster.transform.position;
        float distanceSqr = toTarget.sqrMagnitude;
        float chaseRangeSqr = monster.ChaseRange * monster.ChaseRange;
        float attackRangeSqr = monster.AttackRange * monster.AttackRange;

        if (distanceSqr > chaseRangeSqr)
        {
            monster.ClearTarget();
            stateMachine.ChangeState(MonsterStateType.Patrol);
            return;
        }

        if (distanceSqr <= attackRangeSqr && attackTimer >= monster.AttackDelay)
        {
            stateMachine.ChangeState(MonsterStateType.Attack);
            return;
        }

        UpdateDestination(targetPosition);
    }

    private void UpdateDestination(Vector3 targetPosition)
    {
        bool targetMoved = !hasRequestedDestination ||
            (targetPosition - lastRequestedDestination).sqrMagnitude >=
            DestinationUpdateDistanceSqr;

        // 경로 계산이 진행 중인 동안은 hasPath가 false일 수 있으므로 제외.
        // 목표가 stoppingDistance 밖에 있을 때만 소실·만료된 경로를 복구한다.
        bool needsPath = !monster.Agent.pathPending &&
            (!monster.Agent.hasPath || monster.Agent.isPathStale) &&
            (targetPosition - monster.transform.position).sqrMagnitude >
            monster.Agent.stoppingDistance * monster.Agent.stoppingDistance;

        if (!targetMoved && !needsPath)
            return;

        if (monster.Agent.SetDestination(targetPosition))
        {
            lastRequestedDestination = targetPosition;
            hasRequestedDestination = true;
        }
    }
}
