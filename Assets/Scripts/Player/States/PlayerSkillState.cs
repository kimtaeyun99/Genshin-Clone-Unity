using UnityEngine;

public class PlayerSkillState : PlayerStateBase
{
    public PlayerSkillState(PlayerController player, PlayerStateMachine stateMachine) : base(player,stateMachine)
    {

    }
    public override void Enter()
    {
        player.Animation.SetSkillIndex(1);
        player.Animation.PlaySkill();
    }
    public override void Update()
    {
        
    }
    public override void Exit()
    {
        
    }
}
