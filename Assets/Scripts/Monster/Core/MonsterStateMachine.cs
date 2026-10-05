using System.Collections.Generic;

public class MonsterStateMachine
{
    private readonly Dictionary<MonsterStateType, MonsterStateBase> states = new();

    public MonsterStateBase CurrentState;

    public void AddState(MonsterStateType type, MonsterStateBase state)
    {
        states.Add(type, state);
    }

    public void Initialize(MonsterStateType type)
    {
        CurrentState = states[type];
        CurrentState.Enter();
    }

    public void ChangeState(MonsterStateType newStateType)
    {
        MonsterStateBase newState = states[newStateType];

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