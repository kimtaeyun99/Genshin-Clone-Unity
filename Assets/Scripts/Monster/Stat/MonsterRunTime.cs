using System;
using UnityEngine;

// Unity 컴포넌트가 아닌 순수 런타임 상태 모델.
public sealed class MonsterRunTime
{
    public event Action<float, float> OnHPChanged;
    public event Action OnDead;

    public float MaxHP { get; private set; }
    public float CurrentHP { get; private set; }
    public bool IsDead { get; private set; }

    public MonsterRunTime(float maxHP)
    {
        MaxHP = Mathf.Max(0f, maxHP);
        CurrentHP = MaxHP;
        IsDead = CurrentHP <= 0f;
    }

    public void TakeDamage(float damage)
    {
        if (IsDead || damage <= 0f)
            return;

        float newHP = Mathf.Clamp(CurrentHP - damage, 0f, MaxHP);
        if (newHP == CurrentHP)
            return;

        CurrentHP = newHP;
        OnHPChanged?.Invoke(CurrentHP, MaxHP);

        if (CurrentHP > 0f)
            return;

        IsDead = true;
        OnDead?.Invoke();
    }

    // 살아 있을 때만 전투 이탈 시 체력 완전 회복.
    public void ResetCombatState()
    {
        if (IsDead || CurrentHP == MaxHP)
            return;

        CurrentHP = MaxHP;
        OnHPChanged?.Invoke(CurrentHP, MaxHP);
    }
}
