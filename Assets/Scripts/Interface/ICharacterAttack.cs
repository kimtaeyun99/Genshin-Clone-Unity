public interface ICharacterAttack
{
    void Enter(CharacterRunTime characterRunTime,PlayerAnimationController playerAnimationController);
    void AttackUpdate();
    void Exit();
    void OnAttackInput();

    bool IsFinished { get; }
}
