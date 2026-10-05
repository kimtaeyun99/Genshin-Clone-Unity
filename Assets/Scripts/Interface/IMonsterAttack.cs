using UnityEngine;

public interface IMonsterAttack
{
    void Enter(MonsterController monster);
    void AttackUpdate();
    void Exit();

    bool IsFinished { get; }
}

