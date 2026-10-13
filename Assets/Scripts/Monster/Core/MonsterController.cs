using UnityEngine;
using UnityEngine.AI;

public class MonsterController : MonoBehaviour
{
    [Header("Monster")]
    [SerializeField] private string monsterName;
    [SerializeField] private bool isBoss;

    [Header("Idle")]
    [SerializeField] private float idleTime = 2f;

    [Header("Patrol")]
    [SerializeField] private float patrolRadius = 10f;
    [SerializeField] private float detectionRadius = 8f;

    [Header("Chase")]
    [SerializeField] private float chaseRange = 15f;

    [Header("Attack")]
    [SerializeField] private float attack = 100f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackDelay = 3f;
    [SerializeField] private float attackMultiplier = 1f;

    [Header("Dead")]
    [SerializeField] private float deadDestroyDelay = 3f;

    [Header("Element")]
    [SerializeField] private ElementType elementType;

    public string MonsterName => monsterName;
    public bool IsBoss => isBoss;

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

    public NavMeshAgent Agent { get; private set; }

    public StateMachine<MonsterStateType> StateMachine
    {
        get;
        private set;
    }

    public Transform Target
    {
        get;
        private set;
    }

    public MonsterAnimationController Animation
    {
        get;
        private set;
    }

    public AttackHitbox[] Hitbox
    {
        get;
        private set;
    }

    public MonsterHealth monsterHealth
    {
        get;
        private set;
    }

    public IMonsterAttack MonsterAttack
    {
        get;
        private set;
    }

    public MonsterHealthBar HealthBar
    {
        get;
        private set;
    }

    private void Awake()
    {
        Agent =
            GetComponent<NavMeshAgent>();

        StateMachine =
            new StateMachine<MonsterStateType>();

        Animation =
            GetComponent<MonsterAnimationController>();

        Hitbox =
            GetComponentsInChildren<AttackHitbox>(true);

        monsterHealth =
            GetComponent<MonsterHealth>();

        MonsterAttack =
            GetComponent<IMonsterAttack>();

        HealthBar =
            GetComponentInChildren<MonsterHealthBar>(true);

        InitializeStates();
    }

    private void OnEnable()
    {
        monsterHealth.OnDead += HandleDead;
    }

    private void OnDisable()
    {
        monsterHealth.OnDead -= HandleDead;
    }

    private void Start()
    {
        SpawnPosition = transform.position;

        Agent.stoppingDistance = AttackRange;

        StateMachine.Initialize(
            MonsterStateType.Idle
        );
    }

    private void Update()
    {
        StateMachine.Update();
    }

    private void InitializeStates()
    {
        StateMachine.AddState(
            MonsterStateType.Idle,
            new MonsterIdleState(
                this,
                StateMachine
            )
        );

        StateMachine.AddState(
            MonsterStateType.Patrol,
            new MonsterPatrolState(
                this,
                StateMachine
            )
        );

        StateMachine.AddState(
            MonsterStateType.Chase,
            new MonsterChaseState(
                this,
                StateMachine
            )
        );

        StateMachine.AddState(
            MonsterStateType.Attack,
            new MonsterAttackState(
                this,
                StateMachine
            )
        );

        StateMachine.AddState(
            MonsterStateType.Dead,
            new MonsterDeadState(
                this,
                StateMachine
            )
        );
    }

    public void SetTarget(Transform target)
    {
        Target = target;

        if (isBoss)
        {
            if (BossHealthUI.Instance != null)
            {
                BossHealthUI.Instance.ShowBoss(
                    monsterHealth,
                    monsterName
                );
            }

            return;
        }

        if (HealthBar != null)
        {
            HealthBar.SetVisible(true);
        }
    }

    public void ClearTarget()
    {
        Target = null;

        monsterHealth.ResetCombatState();

        if (isBoss)
        {
            if (BossHealthUI.Instance != null)
            {
                BossHealthUI.Instance.HideBoss();
            }

            return;
        }

        if (HealthBar != null)
        {
            HealthBar.SetVisible(false);
        }
    }

    private void HandleDead()
    {
        // 사망 시 UI도 제거
        if (isBoss)
        {
            if (BossHealthUI.Instance != null)
            {
                BossHealthUI.Instance.HideBoss();
            }
        }
        else
        {
            if (HealthBar != null)
            {
                HealthBar.SetVisible(false);
            }
        }

        StateMachine.ChangeState(
            MonsterStateType.Dead
        );
    }
}