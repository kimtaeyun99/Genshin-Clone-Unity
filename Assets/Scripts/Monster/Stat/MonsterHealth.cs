using UnityEngine;

public class MonsterHealth : MonoBehaviour, IDamageable, IElementApplicable, IAttackable
{
    [Header("HP")]
    [SerializeField] private float maxHP = 100f;
    [SerializeField] private float currentHP;

    [Header("Element")]
    [SerializeField] private ElementType hostElement;

    private void Awake()
    {
        currentHP = maxHP;
        hostElement = ElementType.None;
    }

    public void TakeDamage(float damage)
    {
        Debug.Log($"{damage} 데미지받음");
        currentHP -= damage;

        currentHP = Mathf.Clamp(currentHP, 0, maxHP);

        if(currentHP <= 0f)
        {
            Die();
        }
        Debug.Log(currentHP);
    }
    private void Die()
    {
        Debug.Log("사망");
    }
    public ElementReactionType ApplyElement(ElementType guestElement)
    {
        if(guestElement == ElementType.None || guestElement == ElementType.Physical)
        {
            return ElementReactionType.None;
        }

        if(hostElement == ElementType.None)
        {
            hostElement = guestElement;
            Debug.Log($"부착 : {hostElement}");
            return ElementReactionType.None;
        }
        else if(hostElement == guestElement)
        {
            return ElementReactionType.None;
        }
        else
        {
            ElementReactionType reaction = ElementReaction.GetReaction(hostElement, guestElement);
            hostElement = ElementType.None;
            Debug.Log($"{reaction} 반응");
            return reaction;
        }
    }
    public void TakeAttack(AttackData attackData)
    {
        float damage = attackData.Damage;

        ElementReactionType reaction = ApplyElement(attackData.ElementType);

        float elementDamageMultiplier = ElementReaction.GetElementDamageMultiplier(reaction);

        damage = damage * elementDamageMultiplier;

        TakeDamage(damage);

    }
}
