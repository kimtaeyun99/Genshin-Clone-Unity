using System;
using UnityEngine;

public class MonsterHealth :
    MonoBehaviour,
    IDamageable,
    IElementApplicable,
    IAttackable
{
    [Header("HP")]
    [SerializeField] private float maxHP = 100f;
    [SerializeField] private float currentHP;

    [Header("Element")]
    [SerializeField] private ElementType hostElement;

    [Header("Damage UI")]
    [SerializeField] private float damageUIHeight = 2f;

    public event Action OnDead;
    public float MaxHP => maxHP;
    public float CurrentHP => currentHP;
    public bool IsDead { get; private set; }

    public event Action<float, float> OnHPChanged;

    public event Action<ElementType> OnHostElementChanged;
    public ElementType HostElement => hostElement;
    private void Awake()
    {
        currentHP = maxHP;
        hostElement = ElementType.None;
    }
    public void TakeDamage(float damage)
    {

        currentHP = Mathf.Clamp(
            currentHP - damage,
            0,
            maxHP
        );

        OnHPChanged?.Invoke(
            currentHP,
            maxHP
        );

        if (currentHP <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        IsDead = true;
        OnDead?.Invoke();
    }

    public void TakeAttack(AttackData attackData)
    {
        float damage = attackData.Damage;

        ElementReactionType reaction =
            ApplyElement(attackData.ElementType);

        float elementDamageMultiplier =
            ElementReaction.GetElementDamageMultiplier(
                reaction
            );

        damage *= elementDamageMultiplier;

        TakeDamage(damage);

        if (DamageUIManager.Instance != null)
        {
            DamageUIManager.Instance.ShowDamage(
                damage,
                attackData.ElementType,
                transform.position +
                Vector3.up * damageUIHeight
            );
        }
    }

    public ElementReactionType ApplyElement(ElementType guestElement)
    {
        if (
            guestElement == ElementType.None ||
            guestElement == ElementType.Physical
        )
        {
            return ElementReactionType.None;
        }

        if (hostElement == ElementType.None)
        {
            hostElement = guestElement;

            OnHostElementChanged?.Invoke(hostElement);

            return ElementReactionType.None;
        }

        if (hostElement == guestElement)
        {
            return ElementReactionType.None;
        }

        ElementReactionType reaction =
            ElementReaction.GetReaction(
                hostElement,
                guestElement
            );

        hostElement = ElementType.None;

        OnHostElementChanged?.Invoke(hostElement);

        if (
            reaction != ElementReactionType.None &&
            DamageUIManager.Instance != null
        )
        {
            DamageUIManager.Instance.ShowReaction(
                reaction,
                guestElement,
                transform.position + Vector3.up * damageUIHeight
            );
        }

        return reaction;
    }
    public void ResetCombatState()
    {
        currentHP = maxHP;

        OnHPChanged?.Invoke(
            currentHP,
            maxHP
        );

        if (hostElement != ElementType.None)
        {
            hostElement = ElementType.None;

            OnHostElementChanged?.Invoke(
                hostElement
            );
        }
    }
}