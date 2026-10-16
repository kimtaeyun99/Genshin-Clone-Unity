using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Element")]
    [SerializeField] private ElementType hostElement;

    [Header("Damage UI")]
    [SerializeField] private float damageUIHeight = 2f;

    [Header("Invincible (Player)")]
    [SerializeField] private float switchInvincibleDuration = 0.5f;

    private float invincibleUntil;
    private MonsterRunTime monsterRunTime;

    public event Action OnDead;
    public event Action<ElementType> OnHostElementChanged;

    public ElementType HostElement => hostElement;
    public bool IsInvincible => Time.time < invincibleUntil;

    private void Awake()
    {
        hostElement = ElementType.None;
    }
    public void BindMonsterRunTime(MonsterRunTime runtime)
    {
        monsterRunTime = runtime;
    }

    public void TakeAttack(AttackData attackData)
    {
        if (monsterRunTime != null && monsterRunTime.IsDead)
            return;

        if (IsInvincible || attackData.Damage <= 0f)
            return;

        float damage = attackData.Damage;
        ElementReactionType reaction = ApplyElement(attackData.ElementType);
        damage *= ElementReaction.GetElementDamageMultiplier(reaction);

        if (IsInvincible || damage <= 0f)
            return;

        if (monsterRunTime != null)
        {
            if (monsterRunTime.IsDead)
                return;

            monsterRunTime.TakeDamage(damage);
        }
        else
        {
            CharacterRunTime character =
                CharacterManager.Instance?.CurrentCharacter;

            if (character == null || character.IsDead)
                return;

            character.TakeDamage(damage);

            if (character.IsDead)
                OnDead?.Invoke();
        }
        if (DamageUIManager.Instance != null)
        {
            DamageUIManager.Instance.ShowDamage(
                damage,
                attackData.ElementType,
                transform.position + Vector3.up * damageUIHeight
            );
        }
    }

    public ElementReactionType ApplyElement(ElementType guestElement)
    {
        if (monsterRunTime != null && monsterRunTime.IsDead)
            return ElementReactionType.None;

        if (guestElement == ElementType.None ||
            guestElement == ElementType.Physical)
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
            return ElementReactionType.None;

        ElementReactionType reaction = ElementReaction.GetReaction(
            hostElement, guestElement);

        hostElement = ElementType.None;
        OnHostElementChanged?.Invoke(hostElement);

        if (reaction != ElementReactionType.None &&
            DamageUIManager.Instance != null)
        {
            DamageUIManager.Instance.ShowReaction(
                reaction,
                guestElement,
                transform.position + Vector3.up * damageUIHeight
            );
        }

        return reaction;
    }

    public void ResetElement()
    {
        if (hostElement == ElementType.None)
            return;

        hostElement = ElementType.None;
        OnHostElementChanged?.Invoke(hostElement);
    }

    public void StartSwitchInvincible()
    {
        invincibleUntil = Time.time + switchInvincibleDuration;
    }

    public void StartDodgeInvincible(float duration)
    {
        invincibleUntil = Mathf.Max(invincibleUntil, Time.time + duration);
    }
}
