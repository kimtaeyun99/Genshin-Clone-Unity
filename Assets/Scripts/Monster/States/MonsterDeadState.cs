using UnityEngine;

public class MonsterDeadState : MonsterStateBase
{
    public MonsterDeadState(MonsterController monster,StateMachine<MonsterStateType> stateMachine) : base(monster, stateMachine)
    {
    }
    public override void Enter()
    {
        monster.Agent.ResetPath();
        monster.Agent.enabled = false;
        monster.ClearTarget(false);
        monster.BodyCollider.enabled = false;

        monster.MonsterAnimationController.PlayDead();
        Object.Destroy(monster.gameObject, monster.DeadDestroyDelay);
    }
}
