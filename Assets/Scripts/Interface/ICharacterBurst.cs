public interface ICharacterBurst
{
    void Enter(CharacterRunTime characterRunTime, PlayerAnimationController playerAnimationController);
    void BurstUpdate();
    void Exit();
    bool IsFinished { get; }
}
