using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable,IElementApplicable,IAttackable
{
    [Header("Element")]
    [SerializeField] private ElementType hostElement;

    public event Action OnDead;

    private CharacterRunTime currentCharacter;

    private void Awake()
    {
        hostElement = ElementType.None;
    }
    public void TakeDamage(float damage)
    {
        currentCharacter = CharacterManager.Instance.CurrentCharacter;

        currentCharacter.TakeDamage(damage);

        if(currentCharacter.IsDead)
        {
            OnDead?.Invoke();
        }
        Debug.Log($"{currentCharacter.Data.CharacterName} 피격 : {damage} / " + $"현재 HP : {currentCharacter.CurrentHP}");
    }
    public void TakeAttack(AttackData attackData)
    {
        float damage = attackData.Damage;

        ElementReactionType reaction = ApplyElement(attackData.ElementType);

        float elementDamageMultiplier = ElementReaction.GetElementDamageMultiplier(reaction);

        damage = damage * elementDamageMultiplier;

        TakeDamage(damage);

    }
    public ElementReactionType ApplyElement(ElementType guestElement)
    {
        if (guestElement == ElementType.None || guestElement == ElementType.Physical)
        {
            return ElementReactionType.None;
        }

        if (hostElement == ElementType.None)
        {
            hostElement = guestElement;
            Debug.Log($"부착 : {hostElement}");
            return ElementReactionType.None;
        }
        else if (hostElement == guestElement)
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
    public void ResetElement()
    {
        hostElement = ElementType.None;
    }
}