using UnityEngine;
public class MonsterAttackState : MonsterStateBase
{
    private IMonsterAttack currentAttack;

    public MonsterAttackState(
        MonsterController monster,
        MonsterStateMachine stateMachine)
        : base(monster, stateMachine)
    {
    }

    public override void Enter()
    {
        Debug.Log("Monster Attack State Enter 시작");

        monster.Agent.ResetPath();

        currentAttack = monster.MonsterAttack;

        Debug.Log($"MonsterAttack : {currentAttack}");

        currentAttack.Enter(monster);

        Debug.Log("Monster Attack 진입");
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
        Debug.Log("Monster Attack 종료");
        currentAttack?.Exit();
        currentAttack = null;
    }
}