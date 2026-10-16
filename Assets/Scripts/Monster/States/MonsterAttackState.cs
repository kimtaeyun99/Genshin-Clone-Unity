using UnityEngine;
public class MonsterAttackState : MonsterStateBase
{
    private IMonsterAttack currentAttack;

    public MonsterAttackState(
        MonsterController monster,
        StateMachine<MonsterStateType> stateMachine)
        : base(monster, stateMachine)
    {
    }

    public override void Enter()
    {
        monster.Agent.isStopped = true;
        monster.Agent.ResetPath();
        monster.Agent.velocity = Vector3.zero;

        currentAttack = monster.MonsterAttack;

        currentAttack.Enter(monster);
    }

    public override void Update()
    {
        if (monster.Target == null)
        {
            stateMachine.ChangeState(MonsterStateType.Patrol);
            return;
        }

        currentAttack.AttackUpdate();

        if (currentAttack.IsFinished)
        {
            stateMachine.ChangeState(MonsterStateType.Chase);
        }
    }

    public override void Exit()
    {
        currentAttack?.Exit();
        currentAttack = null;
    }
}