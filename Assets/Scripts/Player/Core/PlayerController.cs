using Polyart;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PlayerStateMachine StateMachine { get; private set; }
    public PlayerMovement Movement { get; private set; }

    public PlayerStamina Stamina { get; private set; }
    public PlayerAnimationController Animation { get; private set; }
    [SerializeField] private CharacterParty characterParty;

    [SerializeField] private CharacterManager characterManager;

    public CharacterManager CharacterManager => characterManager;
    private void Awake()
    {
        Movement = GetComponent<PlayerMovement>();

        Stamina = GetComponent<PlayerStamina>();

        Animation = GetComponent<PlayerAnimationController>();

        StateMachine = new PlayerStateMachine();

        InitializeStates();
    }
    private void OnEnable()
    {
        InputManager.Instance.OnSwitchCharacter += SwitchCharacter;
    }

    private void OnDisable()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnSwitchCharacter -= SwitchCharacter;
        }
    }

    private void Start()
    {
        characterParty.SelectCharacter(0);
        StateMachine.Initialize(PlayerStateType.Idle);
    }

    private void Update()
    {
        Movement.CheckGround();

        Movement.ApplyGravity();

        StateMachine.Update();
    }

    private void InitializeStates()
    {
        StateMachine.AddState(PlayerStateType.Idle,new PlayerIdleState(this, StateMachine));

        StateMachine.AddState(PlayerStateType.Move,new PlayerMoveState(this, StateMachine));

        StateMachine.AddState(PlayerStateType.Jump,new PlayerJumpState(this, StateMachine));

        StateMachine.AddState(PlayerStateType.Fall,new PlayerFallState(this, StateMachine));

        StateMachine.AddState(PlayerStateType.Dodge,new PlayerDodgeState(this, StateMachine));

        //StateMachine.AddState(PlayerStateType.Climb, new PlayerClimbState(this, StateMachine));

        StateMachine.AddState(PlayerStateType.Attack, new PlayerAttackState(this, StateMachine));

        StateMachine.AddState(PlayerStateType.Skill, new PlayerSkillState(this, StateMachine));

        StateMachine.AddState(PlayerStateType.Burst, new PlayerBurstState(this, StateMachine));
    }

    private void SwitchCharacter(int index)
    {
        int partyIndex = index - 1;

        if (partyIndex < 0 || partyIndex >= characterParty.Count)
        {
            return;
        }

        if (partyIndex == characterParty.CurrentIndex)
        {
            return;
        }
        StateMachine.ChangeState(PlayerStateType.Idle);

        characterParty.SelectCharacter(partyIndex);
    }
}