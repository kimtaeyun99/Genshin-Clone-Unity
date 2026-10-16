using UnityEngine;

public class MonsterChaseState : MonsterStateBase
{
    private const float DestinationUpdateDistance = 0.05f;
    private const float DestinationUpdateDistanceSqr = DestinationUpdateDistance * DestinationUpdateDistance;

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
        monster.MonsterAnimationController.PlayChase();

        attackTimer = 0f;
        hasRequestedDestination = false;

        monster.Agent.stoppingDistance = monster.AttackRange;
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

        if (distanceSqr <= attackRangeSqr && attackTimer >= monster.AttackDelay && monster.MonsterAttack != null)
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
