public abstract class MonsterStateBase : State
{
    protected MonsterController monster;

    protected StateMachine<MonsterStateType> stateMachine;

    protected MonsterStateBase(MonsterController monster, StateMachine<MonsterStateType> stateMachine)
    {
        this.monster = monster;
        this.stateMachine = stateMachine;
    }
}
