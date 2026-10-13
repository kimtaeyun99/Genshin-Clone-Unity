using UnityEngine;

public class PlayerSkillState : PlayerStateBase
{
    private ICharacterSkill currentSkill;

    public PlayerSkillState(
        PlayerController player,
        StateMachine<PlayerStateType> stateMachine)
        : base(player, stateMachine)
    {
    }

    public override void Enter()
    {
        currentSkill = player.CharacterManager.CurrentSkill;

        if (currentSkill == null)
        {
            ChangeNextState();
            return;
        }

        currentSkill.Enter(
            player.CharacterManager.CurrentCharacter,
            player.Animation
        );

        InputManager.Instance.OnSkill += OnSkill;
    }

    public override void Update()
    {
        if (currentSkill == null)
        {
            return;
        }

        currentSkill.SkillUpdate();

        if (currentSkill.IsFinished)
        {
            ChangeNextState();
        }
    }

    public override void Exit()
    {
        InputManager.Instance.OnSkill -= OnSkill;

        currentSkill?.Exit();

        CharacterRunTime character = player.CharacterManager.CurrentCharacter;

        if(character != null)
        {
            character.StartSkillCoolDown(character.Data.SkillCooldown);
        }

        currentSkill = null;

    }

    private void OnSkill()
    {
        currentSkill?.OnSkillInput();
    }
}