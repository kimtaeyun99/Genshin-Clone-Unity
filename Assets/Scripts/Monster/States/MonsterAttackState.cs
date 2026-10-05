using UnityEngine;

public class MonsterAttackState : MonsterStateBase
{
    private bool attackStarted;

    public MonsterAttackState(
        MonsterController monster,
        MonsterStateMachine stateMachine)
        : base(monster, stateMachine)
    {
    }

    public override void Enter()
    {
        monster.Agent.ResetPath();

        attackStarted = false;

        LookTarget();

        int attackIndex = Random.Range(0, 2);

        monster.Animation.PlayAttack(attackIndex);

        Debug.Log("Monster Attack 진입");
    }

    public override void Update()
    {
        if (monster.Target == null)
        {
            stateMachine.ChangeState(MonsterStateType.Idle);
            return;
        }

        // Animator가 Attack으로 전환되는 동안 기다림
        if (monster.Animation.IsTransition())
        {
            return;
        }

        // 전환이 끝났으면 Attack 애니메이션 시작된 것으로 처리
        if (!attackStarted)
        {
            attackStarted = true;
            return;
        }

        AnimatorStateInfo stateInfo =
            monster.Animation.GetCurrentStateInfo();

        // 현재 공격 애니메이션이 끝남
        if (stateInfo.normalizedTime >= 1f)
        {
            stateMachine.ChangeState(MonsterStateType.Chase);
        }
    }

    public override void Exit()
    {
        Debug.Log("Monster Attack 종료");
    }

    private void LookTarget()
    {
        Vector3 direction =
            monster.Target.position - monster.transform.position;

        direction.y = 0f;

        if (direction.sqrMagnitude <= 0.01f)
        {
            return;
        }

        monster.transform.rotation =
            Quaternion.LookRotation(direction);
    }
}