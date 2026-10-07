using System.Collections.Generic;
using UnityEngine;

public class BossMonsterAttack : MonoBehaviour, IMonsterAttack
{
    private MonsterController monster;

    private bool attackStarted;
    private bool isFinished;
    public bool IsFinished => isFinished;
    private AttackHitbox[] hitboxes;
    private readonly HashSet<IAttackable> hitTargets = new();

    public void Enter(MonsterController monster)
    {
        this.monster = monster;

        hitboxes = GetComponentsInChildren<AttackHitbox>(true);

        attackStarted = false;
        isFinished = false;

        hitTargets.Clear();

        foreach (AttackHitbox hitbox in hitboxes)
        {
            hitbox.SetSharedHitTargets(hitTargets);

            hitbox.SetAttack(
                monster.Attack,
                monster.AttackMultiplier,
                monster.ElementType
            );
        }

        LookTarget();

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
        DisableHitboxes();

        foreach(AttackHitbox hitbox in hitboxes)
        {
            hitbox.ClearSharedHitTargets();
        }

        hitTargets.Clear();
        Debug.Log("Attack≈¿Â");
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
    private void EnableHitboxes()
    {
        foreach(AttackHitbox hitbox in hitboxes)
        {
            hitbox.gameObject.SetActive(true);
        }
    }
    private void DisableHitboxes()
    {
        foreach(AttackHitbox hitbox in hitboxes)
        {
            hitbox.gameObject.SetActive(false);
            hitbox.ClearHitbox();
        }
    }
}