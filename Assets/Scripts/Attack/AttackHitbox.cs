using System.Collections.Generic;
using UnityEngine;
public class AttackHitbox : MonoBehaviour
{
    private float damage;
    private ElementType element;

    private readonly HashSet<IAttackable> hitTargets = new();
    public float Damage => damage;
    public ElementType Element => element;

    public void SetAttack(float atk, float multiplier, ElementType elementType)
    {
        damage = atk * multiplier;
        element = elementType;
    }
    public void ClearHitbox()
    {
        hitTargets.Clear();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IAttackable>(out IAttackable attackable))
        {
            if(hitTargets.Contains(attackable))
            {
                return;
            }

            hitTargets.Add(attackable);

            AttackData attackData = new AttackData(damage, element);
            attackable.TakeAttack(attackData);
        }
    }
}
