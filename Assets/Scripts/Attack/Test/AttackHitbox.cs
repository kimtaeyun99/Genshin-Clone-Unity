using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    [SerializeField] private float damage = 10f;

    [SerializeField] private ElementType element = ElementType.Pyro;
    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<IAttackable>(out IAttackable attackable))
        {
            AttackData attackData = new AttackData(damage, element);

            attackable.TakeAttack(attackData);
        }
    }
}
