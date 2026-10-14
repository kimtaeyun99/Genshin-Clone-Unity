public class PlayerSkillState : PlayerStateBase
{
    private ICharacterSkill currentSkill;
    // 캐릭터 교체 시에는 CharacterManager.CurrentCharacter가 이미 새 캐릭터다.
    // 따라서 스킬을 시작한 캐릭터를 별도로 보관해야 올바른 대상에 쿨다운이 적용된다.
    private CharacterRunTime skillOwner;

    public PlayerSkillState(
        PlayerController player,
        StateMachine<PlayerStateType> stateMachine)
        : base(player, stateMachine)
    {
    }

    public override void Enter()
    {
        CharacterManager manager = CharacterManager.Instance;
        currentSkill = manager != null ? manager.CurrentSkill : null;
        skillOwner = manager != null ? manager.CurrentCharacter : null;

        if (currentSkill == null || skillOwner == null)
        {
            currentSkill = null;
            skillOwner = null;
            ChangeNextState();
            return;
        }

        currentSkill.Enter(skillOwner, player.Animation);
        InputManager.Instance.OnSkill += OnSkill;
    }

    public override void Update()
    {
        if (currentSkill == null)
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

            if (skillOwner != null)
                skillOwner.StartSkillCoolDown(skillOwner.Data.SkillCooldown);
        }

        currentSkill = null;
        skillOwner = null;
    }

    private void OnSkill()
    {
        currentSkill?.OnSkillInput();
    }
}
