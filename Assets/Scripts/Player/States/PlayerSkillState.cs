public class PlayerSkillState : PlayerStateBase
{
    private ICharacterSkill currentSkill;
    private CharacterRunTime character;

    public PlayerSkillState(
        PlayerController player,
        StateMachine<PlayerStateType> stateMachine)
        : base(player, stateMachine)
    {
    }

    public override void Enter()
    {
		character = CharacterManager.Instance.CurrentCharacter;
		currentSkill = CharacterManager.Instance.CurrentSkill;

		if (currentSkill == null || character == null)
        {
            ChangeNextState();
            return;
        }

        currentSkill.Enter(character, player.PlayerAnimationController);

        InputManager.Instance.OnSkill += OnSkill;
    }

    public override void Update()
    {
        if (currentSkill == null)
            return;

        if (player.PlayerAnimationController.IsTransition())
            return;

        currentSkill.SkillUpdate();

        if (currentSkill.IsFinished)
            ChangeNextState();
    }

    public override void Exit()
    {
        InputManager.Instance.OnSkill -= OnSkill;

        if (currentSkill != null)
        {
            currentSkill.Exit();

            if (character != null)
                character.StartSkillCoolDown(character.Data.SkillCoolDown);
        }

        currentSkill = null;
        character = null;
    }

    private void OnSkill()
    {
        currentSkill?.OnSkillInput();
    }
}
