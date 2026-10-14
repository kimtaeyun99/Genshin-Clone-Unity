using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Character Switch")]
    [SerializeField] private float switchCoolDown = 1f;
    [SerializeField] private float switchCoolDownTimer;

    // The party UI only needs updates while the switch cooldown changes.
    public event Action<float, float> OnSwitchCooldownChanged;

    public float SwitchCoolDownTimer => switchCoolDownTimer;
    public float SwitchCoolDown => switchCoolDown;
    public bool CanSwitchCharacter => switchCoolDownTimer <= 0f;
    public StateMachine<PlayerStateType> StateMachine { get; private set; }
    public PlayerMovement Movement { get; private set; }

    public PlayerStamina Stamina { get; private set; }
    public PlayerAnimationController Animation { get; private set; }
    // 이전 State 코드의 player.CharacterManager 사용과 호환성을 유지하되,
    // 실제 조회는 싱글톤으로 통일한다. Inspector 참조는 필요하지 않다.
    public CharacterManager CharacterManager => CharacterManager.Instance;
    public Health Health { get; private set; }

    private void Awake()
    {
        Movement = GetComponent<PlayerMovement>();
        Stamina = GetComponent<PlayerStamina>();
        Animation = GetComponent<PlayerAnimationController>();
        Health = GetComponent<Health>();

        StateMachine = new StateMachine<PlayerStateType>();
        InitializeStates();
    }

    private void OnEnable()
    {
        InputManager.Instance.OnSwitchCharacter += SwitchCharacter;
        Health.OnDead += HandleDead;
    }

    private void OnDisable()
    {
        InputManager.Instance.OnSwitchCharacter -= SwitchCharacter;
        Health.OnDead -= HandleDead;
    }

    private void Start()
    {
        CharacterParty.Instance.SelectCharacter(0);
        StateMachine.Initialize(PlayerStateType.Idle);
    }

    private void Update()
    {
        UpdateSwitchCoolDown();
        Movement.CheckGround();
        Movement.ApplyGravity();
        StateMachine.Update();
    }

    private void InitializeStates()
    {
        StateMachine.AddState(PlayerStateType.Idle, new PlayerIdleState(this, StateMachine));
        StateMachine.AddState(PlayerStateType.Move, new PlayerMoveState(this, StateMachine));
        StateMachine.AddState(PlayerStateType.Jump, new PlayerJumpState(this, StateMachine));
        StateMachine.AddState(PlayerStateType.Fall, new PlayerFallState(this, StateMachine));
        StateMachine.AddState(PlayerStateType.Dodge, new PlayerDodgeState(this, StateMachine));
        StateMachine.AddState(PlayerStateType.Attack, new PlayerAttackState(this, StateMachine));
        StateMachine.AddState(PlayerStateType.Skill, new PlayerSkillState(this, StateMachine));
        StateMachine.AddState(PlayerStateType.Burst, new PlayerBurstState(this, StateMachine));
        StateMachine.AddState(PlayerStateType.Dead, new PlayerDeadState(this, StateMachine));
    }

    private void SwitchCharacter(int index)
    {
        if (StateMachine.CurrentState is PlayerDeadState)
            return;

        if (!CanSwitchCharacter)
            return;

        CharacterParty party = CharacterParty.Instance;
        if (party == null)
            return;

        int partyIndex = index - 1;
        if (partyIndex < 0 || partyIndex >= party.Count)
            return;

        if (partyIndex == party.CurrentIndex)
            return;

        if (!party.SelectCharacter(partyIndex))
            return;

        StateMachine.ChangeState(PlayerStateType.Idle);

        switchCoolDownTimer = switchCoolDown;
        OnSwitchCooldownChanged?.Invoke(switchCoolDownTimer, switchCoolDown);

        Health.StartSwitchInvincible();
    }

    private void HandleDead()
    {
        StateMachine.ChangeState(PlayerStateType.Dead);
    }

    private void UpdateSwitchCoolDown()
    {
        if (switchCoolDownTimer <= 0f)
            return;

        switchCoolDownTimer -= Time.deltaTime;
        if (switchCoolDownTimer < 0f)
            switchCoolDownTimer = 0f;

        OnSwitchCooldownChanged?.Invoke(switchCoolDownTimer, switchCoolDown);
    }
}
