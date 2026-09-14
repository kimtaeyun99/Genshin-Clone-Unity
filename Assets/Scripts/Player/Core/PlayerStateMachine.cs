using System.Collections.Generic;

public class PlayerStateMachine
{
    private readonly Dictionary<PlayerStateType, PlayerStateBase> states = new();

    public PlayerStateBase CurrentState { get; private set; }

    public void AddState(PlayerStateType stateType, PlayerStateBase state)
    {
        states.Add(stateType, state);
    }

    public void Initialize(PlayerStateType startStateType)
    {
        CurrentState = states[startStateType];
        CurrentState.Enter();
    }

    public void ChangeState(PlayerStateType newStateType)
    {
        PlayerStateBase newState = states[newStateType];

        if (CurrentState == newState) return;

        CurrentState?.Exit();

        CurrentState = newState;
        CurrentState.Enter();
    }

    public void Update()
    {
        CurrentState?.Update();
    }
}