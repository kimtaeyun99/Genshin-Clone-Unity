using System;
using System.Collections.Generic;

public class StateMachine<TKey> where TKey : struct, Enum
{
    private readonly Dictionary<TKey, State> states = new();

    public State CurrentState { get; private set; }
    public TKey CurrentKey { get; private set; }

    public void AddState(TKey key, State state) => states.Add(key, state);

    public void Initialize(TKey startKey)
    {
        CurrentKey = startKey;
        CurrentState = states[startKey];
        CurrentState.Enter();
    }

    public void ChangeState(TKey newKey)
    {
        State newState = states[newKey];

        if (CurrentState == newState) return;

        CurrentState?.Exit();

        CurrentKey = newKey;
        CurrentState = newState;
        CurrentState.Enter();
    }

    public void Update() => CurrentState?.Update();
}