using System.Collections.Generic;
using UnityEngine;

public class BossMonsterAttack : MonoBehaviour, IMonsterAttack
{
    private MonsterController monster;

    private bool attackStarted;
    private bool isFinished;

    private readonly HashSet<IAttackable> hitTargets = new();

    public bool IsFinished => isFinished;

    public void Enter(MonsterController monster)
    {
        this.monster = monster;

        attackStarted = false;
        isFinished = false;

        // 이번 공격의 피격 기록 초기화
        hitTargets.Clear();

        LookTarget();

        foreach (AttackHitbox attackHitbox in monster.Hitbox)
        {
            attackHitbox.ClearHitbox();

            attackHitbox.SetSharedHitTargets(hitTargets);

            attackHitbox.SetAttack(
                monster.Attack,
                monster.AttackMultiplier,
                monster.ElementType
            );

            attackHitbox.gameObject.SetActive(true);
        }

        int attackIndex = Random.Range(0, 3);

        monster.Animation.PlayAttack(attackIndex);
    }

    public void AttackUpdate()
    {
        if (monster.Animation.IsTransition())
        {
            return;
        }

        if (!attackStarted)
        {
            attackStarted = true;
            return;
        }

        AnimatorStateInfo stateInfo =
            monster.Animation.GetCurrentStateInfo();

        if (stateInfo.normalizedTime >= 1f)
        {
            isFinished = true;
        }
    }

    public void Exit()
    {
        foreach (AttackHitbox attackHitbox in monster.Hitbox)
        {
            attackHitbox.ClearHitbox();
            attackHitbox.ClearSharedHitTargets();

            attackHitbox.gameObject.SetActive(false);
        }

        hitTargets.Clear();

        Debug.Log("Attack 퇴장");
    }

    private void LookTarget()
    {
        if (monster.Target == null)
        {
            return;
        }

        Vector3 direction =
            monster.Target.position - monster.transform.position;

        direction.y = 0f;

        if (direction.sqrMagnitude <= 0.01f)
        {
            return;
        }

        monster.transform.rotation =
            Quaternion.LookRotation(direction);
    }
}