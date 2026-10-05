using UnityEngine;
using UnityEngine.AI;

public class MonsterController : MonoBehaviour
{
    [Header("Idle")]
    [SerializeField] private float idleTime = 2f;

    [Header("Patrol")]
    [SerializeField] private float patrolRadius = 10f;
    [SerializeField] private float detectionRadius = 8f;

    [Header("Chase")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float chaseRange = 15f;

    [Header("Attack")]
    [SerializeField] private float attack = 100f;
    [SerializeField] private float attackMultiplier = 1f;

    public float IdleTime => idleTime;
    public float PatrolRadius => patrolRadius;
    public float DetectionRadius => detectionRadius;
    public float AttackRange => attackRange;
    public float ChaseRange => chaseRange;
    public float Attack => attack;
    public float AttackMultiplier => attackMultiplier;

    public Vector3 SpawnPosition { get; private set; }
    public NavMeshAgent Agent { get; private set; }
    public MonsterStateMachine StateMachine { get; private set; }
    public Transform Target { get; private set; }
    public MonsterAnimationController Animation { get; private set; }
    public AttackHitbox Hitbox { get; private set; }

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();

        StateMachine = new MonsterStateMachine();

        Animation = GetComponent<MonsterAnimationController>();

        Hitbox = GetComponentInChildren<AttackHitbox>(true);

        InitializeStates();
    }

    private void Start()
    {
        SpawnPosition = transform.position;
        StateMachine.Initialize(MonsterStateType.Idle);
    }

    private void Update()
    {
        StateMachine.Update();
    }

    private void InitializeStates()
    {
        StateMachine.AddState(
            MonsterStateType.Idle,
            new MonsterIdleState(this, StateMachine)
        );

        StateMachine.AddState(
            MonsterStateType.Patrol,
            new MonsterPatrolState(this, StateMachine)
        );

        StateMachine.AddState(
            MonsterStateType.Chase,
            new MonsterChaseState(this, StateMachine)
        );

        StateMachine.AddState(
            MonsterStateType.Attack,
            new MonsterAttackState(this, StateMachine)
        );

        StateMachine.AddState(
            MonsterStateType.Dead,
            new MonsterDeadState(this, StateMachine)
        );
    }
    public void SetTarget(Transform target)
    {
        Target = target;
    }
    public void ClearTarget()
    {
        Target = null;
    }
}