using System.Collections.Generic;
using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    [SerializeField] private LayerMask targetLayer;

    private float damage;
    private ElementType element;

    private readonly HashSet<IAttackable> hitTargets = new();

    private HashSet<IAttackable> sharedHitTargets;

    public float Damage => damage;
    public ElementType Element => element;

    public void SetAttack(
        float atk,
        float multiplier,
        ElementType elementType)
    {
        damage = atk * multiplier;
        element = elementType;
    }

    public void SetSharedHitTargets(
        HashSet<IAttackable> sharedHitTargets)
    {
        this.sharedHitTargets = sharedHitTargets;
    }

    public void ClearSharedHitTargets()
    {
        sharedHitTargets = null;
    }

    public void ClearHitbox()
    {
        hitTargets.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((targetLayer.value & (1 << other.gameObject.layer)) == 0)
        {
            return;
        }

        if (!other.TryGetComponent<IAttackable>(
                out IAttackable attackable))
        {
            return;
        }

        HashSet<IAttackable> targets = sharedHitTargets ?? hitTargets;

        if (targets.Contains(attackable))
        {
            return;
        }

        targets.Add(attackable);

        AttackData attackData =
            new AttackData(damage, element);

        attackable.TakeAttack(attackData);
    }
}