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
    private void Awake()
    {
        currentHP = maxHP;
        hostElement = ElementType.None;
    }
    public void TakeDamage(float damage)
    {
        Debug.Log($"{damage} 데미지받음");

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

        Debug.Log(currentHP);
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

    public ElementReactionType ApplyElement(
        ElementType guestElement)
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

            Debug.Log($"부착 : {hostElement}");

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

        Debug.Log(
            $"원소 반응 : Host={hostElement}, " +
            $"Guest={guestElement}, " +
            $"Reaction={reaction}"
        );

        hostElement = ElementType.None;

        if (
            reaction != ElementReactionType.None &&
            DamageUIManager.Instance != null
        )
        {
            DamageUIManager.Instance.ShowReaction(
                reaction,
                guestElement,
                transform.position +
                Vector3.up * damageUIHeight
            );
        }

        return reaction;
    }
}