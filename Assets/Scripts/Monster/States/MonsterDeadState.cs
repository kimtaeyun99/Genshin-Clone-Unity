using UnityEngine;

public class MonsterDeadState : MonsterStateBase
{
    public MonsterDeadState(MonsterController monster,MonsterStateMachine stateMachine) : base(monster, stateMachine)
    {
    }
    public override void Enter()
    {
        monster.Agent.ResetPath();
        monster.Agent.enabled = false;
        monster.ClearTarget();

        foreach(Collider col in monster.GetComponentsInChildren<Collider>())
        {
            col.enabled = false;
        }
        monster.Animation.PlayDead();
        Object.Destroy(monster.gameObject, monster.DeadDestroyDelay);
    }
}
