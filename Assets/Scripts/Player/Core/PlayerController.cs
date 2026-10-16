using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Character Switch")]
    [SerializeField] private float switchCoolDown = 1f;
    [SerializeField] private float switchCoolDownTimer;
    [SerializeField] private PlayerMovement movement;
    [SerializeField] private PlayerStamina stamina;
    [SerializeField] private PlayerAnimationController playerAnimationController;
    [SerializeField] private Health health;

    public float SwitchCoolDown => switchCoolDown;
    public float SwitchCoolDownTimer => switchCoolDownTimer;
    public bool CanSwitchCharacter => switchCoolDownTimer <= 0f;
    public PlayerMovement Movement => movement;
    public PlayerStamina Stamina => stamina;
    public PlayerAnimationController PlayerAnimationController => playerAnimationController;
    public Health Health => health;
    public StateMachine<PlayerStateType> StateMachine { get; private set; }

    public event Action<float, float> OnSwitchCooldownChanged;

    private void Awake()
    {
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
        if (CharacterManager.Instance.CurrentCharacter == null)
        {
            CharacterParty.Instance.SelectCharacter(0);
        }
        if (StateMachine.CurrentState == null)
        {
            StateMachine.Initialize(PlayerStateType.Idle);
        }
        SaveManager.Instance?.RegisterPlayer(this);
    }

    private void Update()
    {
        UpdateSwitchCoolDown();
        Movement.CheckGround();
        Movement.ApplyGravity();
        StateMachine.Update();
        Debug.Log(
    $"State: {StateMachine.CurrentKey}, " +
    $"IsGrounded: {Movement.IsGrounded}"
);
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
        {
            return;
        }

        if (!CanSwitchCharacter)
        {
            return;
        }

        if (CharacterParty.Instance == null)
        {
            return;
        }

        int partyIndex = index - 1;
        if (partyIndex < 0 || partyIndex >= CharacterParty.Instance.Count)
        {
            return;
        }

        if (partyIndex == CharacterParty.Instance.CurrentIndex)
        {
            return;
        }

        if (!CharacterParty.Instance.SelectCharacter(partyIndex))
        {
            return;
        }

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
        {
            return;
        }

        switchCoolDownTimer -= Time.deltaTime;
        if (switchCoolDownTimer < 0f)
        {
            switchCoolDownTimer = 0f;
        }

        OnSwitchCooldownChanged?.Invoke(switchCoolDownTimer, switchCoolDown);
    }
}
