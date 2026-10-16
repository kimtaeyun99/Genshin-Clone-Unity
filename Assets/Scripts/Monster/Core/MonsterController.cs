using UnityEngine;
using UnityEngine.AI;

public class MonsterController : MonoBehaviour
{
    [Header("Monster")]
    [SerializeField] private MonsterData data;
    [SerializeField] private bool isBoss;

    [SerializeField] private Collider bodyCollider;

    [Header("Idle")]
    [SerializeField] private float idleTime = 2f;

    [Header("Patrol")]
    [SerializeField] private float patrolRadius = 10f;
    [SerializeField] private float detectionRadius = 8f;

    [Header("Chase")]
    [SerializeField] private float chaseRange = 15f;

    [Header("Attack")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackDelay = 3f;
    [SerializeField] private float attackMultiplier = 1f;

    [Header("Dead")]
    [SerializeField] private float deadDestroyDelay = 3f;

    [Header("NavMesh")]
    [SerializeField] private NavMeshAgent agent;

    [Header("Animation")]
    [SerializeField] private MonsterAnimationController monsterAnimationController;

    [Header("Health")]
    [SerializeField] private Health health;
    [SerializeField] private MonsterHealthBar healthBar;

    public MonsterData Data => data;
    public bool IsBoss => isBoss;
    public Collider BodyCollider => bodyCollider;
    public float IdleTime => idleTime;
    public float PatrolRadius => patrolRadius;
    public float DetectionRadius => detectionRadius;
    public float ChaseRange => chaseRange;
    public float AttackRange => attackRange;
    public float AttackDelay => attackDelay;
    public float AttackMultiplier => attackMultiplier;

    public float DeadDestroyDelay => deadDestroyDelay;
    public NavMeshAgent Agent => agent;
    public MonsterAnimationController MonsterAnimationController => monsterAnimationController;
    public Health Health => health;
    public MonsterHealthBar HealthBar => healthBar;
    public StateMachine<MonsterStateType> StateMachine { get; private set; }
    public Transform Target { get; private set; }
    public IMonsterAttack MonsterAttack { get; private set; }
    public MonsterRunTime Runtime { get; private set; }
    public Vector3 SpawnPosition { get; private set; }
    public string MonsterName => data.MonsterName;

    private void Awake()
    {
        StateMachine = new StateMachine<MonsterStateType>();
        Runtime = new MonsterRunTime(data);
        MonsterAttack = GetComponent<IMonsterAttack>();
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
                BossHealthUI.Instance.ShowBoss(Runtime, Health, MonsterName);
            return;
        }

        if (HealthBar != null)
            HealthBar.SetVisible(true);
    }

    public void ClearTarget(bool hideUI = true)
    {
        Target = null;

        if (!Runtime.IsDead)
            Runtime.ResetCombatState();

        Health.ResetElement();

        if (!hideUI)
            return;

        if (isBoss)
        {
            if (BossHealthUI.Instance != null)
                BossHealthUI.Instance.HideBoss(Runtime);
        }
        else if (HealthBar != null)
        {
            HealthBar.SetVisible(false);
        }
    }

    private void HandleDead(MonsterRunTime runTime)
    {
        if (isBoss)
        {
            if (BossHealthUI.Instance != null)
                BossHealthUI.Instance.HideElementIcon(runTime);
        }
        else if (HealthBar != null)
        {
            HealthBar.HideElementIcon();
        }

        StateMachine.ChangeState(MonsterStateType.Dead);
    }
    private void OnDestroy()
    {
        if (!isBoss)
            return;

        if (BossHealthUI.Instance != null)
            BossHealthUI.Instance.HideBoss(Runtime);
    }
}
