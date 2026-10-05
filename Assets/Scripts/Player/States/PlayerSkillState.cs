using UnityEngine;

public class PlayerSkillState : PlayerStateBase
{
    private ICharacterSkill currentSkill;

    public PlayerSkillState(
        PlayerController player,
        PlayerStateMachine stateMachine)
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

        Debug.Log("Skill ÁøÀÔ");
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

        currentSkill = null;

        Debug.Log("Skill ÅðÀå");
    }

    private void OnSkill()
    {
        currentSkill?.OnSkillInput();
    }

    private void ChangeNextState()
    {
        if (InputManager.Instance.MoveInput.sqrMagnitude > 0.01f)
        {
            stateMachine.ChangeState(PlayerStateType.Move);
        }
        else
        {
            stateMachine.ChangeState(PlayerStateType.Idle);
        }
    }
}