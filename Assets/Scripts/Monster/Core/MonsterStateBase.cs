public abstract class MonsterStateBase
{
    protected MonsterController monster;
    protected MonsterStateMachine stateMachine;

    protected MonsterStateBase(
        MonsterController monster,
        MonsterStateMachine stateMachine)
    {
        this.monster = monster;
        this.stateMachine = stateMachine;
    }

    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void Exit() { }
}
