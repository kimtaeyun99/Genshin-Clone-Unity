using UnityEngine;

public class MonsterIdleState : MonsterStateBase
{
    private float idleTimer;

    public MonsterIdleState(
        MonsterController monster,
        StateMachine<MonsterStateType> stateMachine)
        : base(monster, stateMachine)
    {
    }

    public override void Enter()
    {
        idleTimer = monster.IdleTime;

        monster.MonsterAnimationController.PlayIdle();
    }

    public override void Update()
    {
        idleTimer -= Time.deltaTime;

        if (idleTimer <= 0f)
        {
            stateMachine.ChangeState(MonsterStateType.Patrol);
        }
    }
}