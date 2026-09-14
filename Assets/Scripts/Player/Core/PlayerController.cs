using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PlayerStateMachine StateMachine { get; private set; }
    public PlayerMovement Movement { get; private set; }

    private void Awake()
    {
        Movement = GetComponent<PlayerMovement>();

        StateMachine = new PlayerStateMachine();

        InitializeStates();
    }

    private void Start()
    {
        StateMachine.Initialize(PlayerStateType.Idle);
    }

    private void Update()
    {
        Movement.CheckGround();

        StateMachine.Update();

        Movement.ApplyGravity();
    }

    private void InitializeStates()
    {
        StateMachine.AddState(
            PlayerStateType.Idle,
            new PlayerIdleState(this, StateMachine)
        );

        StateMachine.AddState(
            PlayerStateType.Move,
            new PlayerMoveState(this, StateMachine)
        );

        StateMachine.AddState(
            PlayerStateType.Jump,
            new PlayerJumpState(this, StateMachine)
        );

        StateMachine.AddState(
            PlayerStateType.Fall,
            new PlayerFallState(this, StateMachine)
        );

        StateMachine.AddState(
            PlayerStateType.Dodge,
            new PlayerDodgeState(this, StateMachine)
        );
    }
}