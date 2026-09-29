using UnityEngine;
public class AttackHitbox : MonoBehaviour
{
    private float damage;
    private ElementType element;

    public float Damage => damage;
    public ElementType Element => element;

    public void SetAttack(float atk, float multiplier, ElementType elementType)
    {
        damage = atk * multiplier;
        element = elementType;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IAttackable>(out IAttackable attackable))
        {
            AttackData attackData = new AttackData(damage, element);

            attackable.TakeAttack(attackData);
        }
    }
}
