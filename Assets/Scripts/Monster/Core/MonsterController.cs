using System.Runtime.ExceptionServices;
using UnityEngine;
using UnityEngine.AI;

public class MonsterController : MonoBehaviour
{
    [Header("Monster")]
    [SerializeField] private string monsterName;
    [SerializeField] private bool isBoss;
    [SerializeField] private float maxHP = 100f;
    [SerializeField] private Collider bodyCollider;

    [Header("Idle")]
    [SerializeField] private float idleTime = 2f;

    [Header("Patrol")]
    [SerializeField] private float patrolRadius = 10f;
    [SerializeField] private float detectionRadius = 8f;

    [Header("Chase")]
    [SerializeField] private float chaseRange = 15f;

    [Header("Attack")]
    [SerializeField] private IMonsterAttack monsterAttack;
    [SerializeField] private float attack = 100f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackDelay = 3f;
    [SerializeField] private float attackMultiplier = 1f;

    [Header("Dead")]
    [SerializeField] private float deadDestroyDelay = 3f;

    [Header("Element")]
    [SerializeField] private ElementType elementType;

    [Header("NavMesh")]
    [SerializeField] private NavMeshAgent agent;

    [Header("Animation")]
    [SerializeField] private MonsterAnimationController monsterAnimationController;

    [Header("Hitbox")]
    [SerializeField] private AttackHitbox[] hitboxes;

    [Header("Health")]
    [SerializeField] private Health health;
    [SerializeField] private MonsterHealthBar healthBar;

    public string MonsterName => monsterName;
    public bool IsBoss => isBoss;
    public Collider BodyCollider => bodyCollider;

    public float IdleTime => idleTime;
    public float PatrolRadius => patrolRadius;
    public float DetectionRadius => detectionRadius;
    public float ChaseRange => chaseRange;

    public float Attack => attack;
    public float AttackRange => attackRange;
    public float AttackDelay => attackDelay;
    public float AttackMultiplier => attackMultiplier;

    public float DeadDestroyDelay => deadDestroyDelay;
    public ElementType ElementType => elementType;

    public Vector3 SpawnPosition { get; private set; }
    public NavMeshAgent Agent => agent;
    public StateMachine<MonsterStateType> StateMachine { get; private set; }
    public Transform Target { get; private set; }
    public MonsterAnimationController MonsterAnimationController => monsterAnimationController;
    public AttackHitbox[] Hitboxes => hitboxes;
    public Health Health => health;
    public MonsterRunTime Runtime { get; private set; }
    public IMonsterAttack MonsterAttack => monsterAttack;
    public MonsterHealthBar HealthBar => healthBar;

    private void Awake()
    {
        StateMachine = new StateMachine<MonsterStateType>();
        Runtime = new MonsterRunTime(maxHP);

        Health.BindMonsterRunTime(Runtime);

        InitializeStates();
    }

    private void OnEnable()
    {
        Runtime.OnDead += HandleDead;
    }

    private void OnDisable()
    {
        Runtime.OnDead -= HandleDead;
    }

    private void Start()
    {
        SpawnPosition = transform.position;
        Agent.stoppingDistance = AttackRange;
        StateMachine.Initialize(MonsterStateType.Idle);
    }

    private void Update()
    {
        StateMachine.Update();
    }

    private void InitializeStates()
    {
        StateMachine.AddState(MonsterStateType.Idle,
            new MonsterIdleState(this, StateMachine));
        StateMachine.AddState(MonsterStateType.Patrol,
            new MonsterPatrolState(this, StateMachine));
        StateMachine.AddState(MonsterStateType.Chase,
            new MonsterChaseState(this, StateMachine));
        StateMachine.AddState(MonsterStateType.Attack,
            new MonsterAttackState(this, StateMachine));
        StateMachine.AddState(MonsterStateType.Dead,
            new MonsterDeadState(this, StateMachine));
    }

    public void SetTarget(Transform target)
    {
        Target = target;

        if (isBoss)
        {
            if (BossHealthUI.Instance != null)
                BossHealthUI.Instance.ShowBoss(Runtime, Health, monsterName);
            return;
        }

        if (HealthBar != null)
            HealthBar.SetVisible(true);
    }

    public void ClearTarget()
    {
        Target = null;

        if (!Runtime.IsDead)
            Runtime.ResetCombatState();

        Health.ResetElement();

        if (isBoss)
        {
            if (BossHealthUI.Instance != null)
                BossHealthUI.Instance.HideBoss();
            return;
        }

        if (HealthBar != null)
            HealthBar.SetVisible(false);
    }

    private void HandleDead()
    {
        if (isBoss)
        {
            if (BossHealthUI.Instance != null)
                BossHealthUI.Instance.HideBoss();
        }
        else if (HealthBar != null)
        {
            HealthBar.SetVisible(false);
        }

        StateMachine.ChangeState(MonsterStateType.Dead);
    }
}
