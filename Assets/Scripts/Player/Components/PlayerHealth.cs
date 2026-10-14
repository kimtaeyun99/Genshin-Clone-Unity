//using System;
//using UnityEngine;

//public class PlayerHealth :
//    MonoBehaviour,
//    IDamageable,
//    IElementApplicable,
//    IAttackable
//{
//    [Header("Element")]
//    [SerializeField] private ElementType hostElement;

//    [Header("Damage UI")]
//    [SerializeField] private float damageUIHeight = 2f;

//    public event Action OnDead;

//    private CharacterRunTime currentCharacter;

//    [Header("Invincible")]
//    [SerializeField] private float switchInvincibleDuration = 0.5f;

//    private float invincibleTimer;

//    public bool IsInvincible => invincibleTimer > 0f;
//    private void Awake()
//    {
//        hostElement = ElementType.None;
//    }
//    private void Update()
//    {
//        if (invincibleTimer > 0f)
//        {
//            invincibleTimer -= Time.deltaTime;

//            if (invincibleTimer < 0f)
//            {
//                invincibleTimer = 0f;
//            }
//        }
//    }
//    public void TakeDamage(float damage)
//    {
//        if (IsInvincible)
//        {
//            return;
//        }

//        currentCharacter =
//            CharacterManager.Instance.CurrentCharacter;

//        currentCharacter.TakeDamage(damage);

//        if (currentCharacter.IsDead)
//        {
//            OnDead?.Invoke();
//        }
//    }

//    public void TakeAttack(AttackData attackData)
//    {
//        if (IsInvincible)
//        {
//            return;
//        }

//        float damage = attackData.Damage;

//        ElementReactionType reaction =
//            ApplyElement(attackData.ElementType);

//        float elementDamageMultiplier =
//            ElementReaction.GetElementDamageMultiplier(
//                reaction
//            );

//        damage *= elementDamageMultiplier;

//        TakeDamage(damage);

//        if (DamageUIManager.Instance != null)
//        {
//            DamageUIManager.Instance.ShowDamage(
//                damage,
//                attackData.ElementType,
//                transform.position +
//                Vector3.up * damageUIHeight
//            );
//        }
//    }

//    public ElementReactionType ApplyElement(
//        ElementType guestElement)
//    {
//        if (
//            guestElement == ElementType.None ||
//            guestElement == ElementType.Physical
//        )
//        {
//            return ElementReactionType.None;
//        }

//        if (hostElement == ElementType.None)
//        {
//            hostElement = guestElement;

//            return ElementReactionType.None;
//        }

//        if (hostElement == guestElement)
//        {
//            return ElementReactionType.None;
//        }

//        ElementReactionType reaction =
//            ElementReaction.GetReaction(
//                hostElement,
//                guestElement
//            );

//        hostElement = ElementType.None;

//        if (
//            reaction != ElementReactionType.None &&
//            DamageUIManager.Instance != null
//        )
//        {
//            DamageUIManager.Instance.ShowReaction(
//                reaction,
//                guestElement,
//                transform.position +
//                Vector3.up * damageUIHeight
//            );
//        }

//        return reaction;
//    }

//    public void ResetElement()
//    {
//        hostElement = ElementType.None;
//    }
//    public void StartSwitchInvincible()
//    {
//        invincibleTimer = switchInvincibleDuration;
//    }
//    public void StartDodgeInvincible(float duration)
//    {
//        invincibleTimer = Mathf.Max(
//            invincibleTimer,
//            duration
//        );
//    }
//}