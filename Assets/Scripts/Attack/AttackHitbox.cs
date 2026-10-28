using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class AttackHitbox : MonoBehaviour
{
    [SerializeField] private LayerMask targetLayer;

    private float damage;
    private ElementType element;

    private readonly HashSet<Health> hitTargets = new();

    public float Damage => damage;
    public ElementType Element => element;
    public event Action OnHit;

    private void OnTriggerEnter(Collider other)
    {
        TryHit(other);
    }
    private void OnTriggerExit(Collider other)
    {
        TryHit(other);
    }
    public void SetAttack(float atk, float multiplier, ElementType elementType)
    {
        damage = atk * multiplier;
        element = elementType;
    }

    public void ClearHitbox()
    {
        hitTargets.Clear();
    }
    private void TryHit(Collider other)
    {
        if ((targetLayer.value & (1 << other.gameObject.layer)) == 0)
            return;

        if (!other.TryGetComponent<Health>(out Health health))
            return;

        if (!hitTargets.Add(health))
            return;


        health.TakeAttack(new AttackData(damage, element));
        OnHit?.Invoke();
    }
}