using UnityEngine;
using UnityEngine.AI;

public class MonsterChaseState : MonsterStateBase
{
    private const float DestinationUpdateDistance = 0.05f;
    private const float DestinationUpdateDistanceSqr =
        DestinationUpdateDistance * DestinationUpdateDistance;

    private const float RangedMoveInterval = 2f;
    private const float RangedStoppingDistance = 0.3f;

    private float attackTimer;
    private float rangedMoveTimer;

    private Vector3 lastRequestedDestination;
    private bool hasRequestedDestination;

    private bool isRanged;
    private int orbitDirection;

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
        rangedMoveTimer = 0f;
        hasRequestedDestination = false;

        isRanged = monster.MonsterAttack is RangedMonsterAttack;
        orbitDirection = Random.value < 0.5f ? -1 : 1;

        monster.Agent.isStopped = false;
        monster.Agent.stoppingDistance =
            isRanged ? RangedStoppingDistance : monster.AttackRange;

        if (monster.Target != null)
        {
            if (isRanged)
                UpdateRangedDestination();
            else
                UpdateDestination(monster.Target.position);
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

        Vector3 targetPosition = monster.Target.position;
        Vector3 offset = targetPosition - monster.transform.position;

        offset.y = 0f;

        float distanceSqr = offset.sqrMagnitude;
        float chaseRangeSqr = monster.ChaseRange * monster.ChaseRange;
        float attackRangeSqr = monster.AttackRange * monster.AttackRange;

        if (distanceSqr > chaseRangeSqr)
        {
            monster.ClearTarget();
            stateMachine.ChangeState(MonsterStateType.Patrol);
            return;
        }

        if (distanceSqr <= attackRangeSqr &&
            attackTimer >= monster.AttackDelay &&
            monster.MonsterAttack != null)
        {
            stateMachine.ChangeState(MonsterStateType.Attack);
            return;
        }

        if (isRanged)
        {
            UpdateRangedMovement(distanceSqr, attackRangeSqr);
        }
        else
        {
            UpdateDestination(targetPosition);
        }
    }

    private void UpdateRangedMovement(
        float distanceSqr,
        float attackRangeSqr)
    {

        if (distanceSqr > attackRangeSqr)
        {
            UpdateDestination(monster.Target.position);
            rangedMoveTimer = 0f;
            return;
        }

        rangedMoveTimer -= Time.deltaTime;

        bool arrived =
            !monster.Agent.pathPending &&
            monster.Agent.hasPath &&
            monster.Agent.remainingDistance <=
                monster.Agent.stoppingDistance + 0.3f;

        if (rangedMoveTimer > 0f && !arrived)
            return;

        UpdateRangedDestination();
        rangedMoveTimer = RangedMoveInterval;
    }

    private void UpdateRangedDestination()
    {
        if (monster.Target == null)
            return;

        Vector3 playerPosition = monster.Target.position;
        Vector3 away = monster.transform.position - playerPosition;

        away.y = 0f;

        if (away.sqrMagnitude < 0.01f)
            away = -monster.transform.forward;

        away.Normalize();

        float minDistance = Mathf.Min(4f, monster.AttackRange * 0.5f);
        float desiredDistance = Mathf.Max(
            minDistance,
            monster.AttackRange * 0.75f
        );

        float angle = Random.Range(45f, 85f) * orbitDirection;

        Vector3 direction =
            Quaternion.Euler(0f, angle, 0f) * away;

        Vector3 destination =
            playerPosition + direction * desiredDistance;

        if (NavMesh.SamplePosition(
            destination,
            out NavMeshHit hit,
            2f,
            monster.Agent.areaMask))
        {
            NavMeshPath path = new NavMeshPath();

            if (monster.Agent.CalculatePath(hit.position, path) &&
                path.status == NavMeshPathStatus.PathComplete)
            {
                monster.Agent.SetDestination(hit.position);

                lastRequestedDestination = hit.position;
                hasRequestedDestination = true;
            }
        }

        if (Random.value < 0.25f)
            orbitDirection *= -1;
    }

    private void UpdateDestination(Vector3 targetPosition)
    {
        bool targetMoved =
            !hasRequestedDestination ||
            (targetPosition - lastRequestedDestination).sqrMagnitude >=
                DestinationUpdateDistanceSqr;

        bool needsPath =
            !monster.Agent.pathPending &&
            (!monster.Agent.hasPath || monster.Agent.isPathStale) &&
            (targetPosition - monster.transform.position).sqrMagnitude >
                monster.Agent.stoppingDistance *
                monster.Agent.stoppingDistance;

        if (!targetMoved && !needsPath)
            return;

        if (monster.Agent.SetDestination(targetPosition))
        {
            lastRequestedDestination = targetPosition;
            hasRequestedDestination = true;
        }
    }
}