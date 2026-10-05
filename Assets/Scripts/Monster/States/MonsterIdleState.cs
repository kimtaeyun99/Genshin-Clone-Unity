using UnityEngine;

public class MonsterIdleState : MonsterStateBase
{
    private float idleTimer;

    public MonsterIdleState(
        MonsterController monster,
        MonsterStateMachine stateMachine)
        : base(monster, stateMachine)
    {
    }

    public override void Enter()
    {
        idleTimer = monster.IdleTime;

        monster.Animation.PlayIdle();

        Debug.Log("Monster Idle 진입");
    }

    public override void Update()
    {
        idleTimer -= Time.deltaTime;

        if (idleTimer <= 0f)
        {
            stateMachine.ChangeState(MonsterStateType.Patrol);
        }
    }

    public override void Exit()
    {
        Debug.Log("Monster Idle 종료");
    }
}