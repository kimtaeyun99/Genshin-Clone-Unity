using System;
using UnityEngine;

// 공통 피격 진입점. HP/사망 상태의 원본 데이터는 각 Runtime이 소유한다.
public class Health : MonoBehaviour
{
    [Header("Element")]
    [SerializeField] private ElementType hostElement;

    [Header("Damage UI")]
    [SerializeField] private float damageUIHeight = 2f;

    [Header("Invincible (Player)")]
    [SerializeField] private float switchInvincibleDuration = 0.5f;

    // 몬스터는 무적 타이머를 사용하지 않는다. 절대 종료 시각 비교로 매 프레임 Update를 제거한다.
    private float invincibleUntil;
    private MonsterRunTime monsterRunTime;

    // 플레이어 사망 상태 진입 알림. 몬스터의 사망 이벤트 원본은 MonsterRunTime.OnDead다.
    public event Action OnDead;
    public event Action<ElementType> OnHostElementChanged;

    public ElementType HostElement => hostElement;
    public bool IsInvincible => Time.time < invincibleUntil;

    private void Awake()
    {
        hostElement = ElementType.None;
    }

    // 몬스터 Controller.Awake에서 생성한 Runtime을 연결한다.
    public void BindMonsterRunTime(MonsterRunTime runtime)
    {
        monsterRunTime = runtime;
    }

    public void TakeDamage(float damage)
    {
        if (IsInvincible || damage <= 0f)
            return;

        if (monsterRunTime != null)
        {
            monsterRunTime.TakeDamage(damage);
            return;
        }

        CharacterManager manager = CharacterManager.Instance;
        if (manager == null)
            return;

        CharacterRunTime character = manager.CurrentCharacter;
        if (character == null || character.IsDead)
            return;

        character.TakeDamage(damage);
        if (character.IsDead)
            OnDead?.Invoke();
    }

    public void TakeAttack(AttackData attackData)
    {
        if (IsInvincible)
            return;

        float damage = attackData.Damage;
        ElementReactionType reaction = ApplyElement(attackData.ElementType);
        damage *= ElementReaction.GetElementDamageMultiplier(reaction);

        TakeDamage(damage);

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

        // 기존 개별 Health 컴포넌트와 같은 원소 소거 규칙.
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
