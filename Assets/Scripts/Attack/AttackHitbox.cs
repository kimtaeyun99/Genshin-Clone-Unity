using System;
using System.Collections.Generic;
using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    [SerializeField] private LayerMask targetLayer;

    private float damage;
    private ElementType element;

    private readonly HashSet<Health> hitTargets = new();
    private HashSet<Health> sharedHitTargets;

    public float Damage => damage;
    public ElementType Element => element;
    public event Action<Health> OnHit;

    public void SetAttack(float atk, float multiplier, ElementType elementType)
    {
        damage = atk * multiplier;
        element = elementType;
    }

    public void SetSharedHitTargets(HashSet<Health> sharedHitTargets)
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

    private void OnTriggerStay(Collider other)
    {
        if ((targetLayer.value & (1 << other.gameObject.layer)) == 0)
            return;

        if (!other.TryGetComponent<Health>(out Health health))
            return;

        HashSet<Health> targets = sharedHitTargets ?? hitTargets;
        if (targets.Contains(health))
            return;

        targets.Add(health);
        health.TakeAttack(new AttackData(damage, element));
        OnHit?.Invoke(health);
    }
}
