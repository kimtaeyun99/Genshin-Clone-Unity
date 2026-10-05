public interface ICharacterSkill
{
    void Enter(CharacterRunTime characterRunTime, PlayerAnimationController playerAnimationController);
    void SkillUpdate();
    void Exit();
    void OnSkillInput();
    bool IsFinished { get; }
}
