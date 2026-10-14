using System;
using UnityEngine;

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

    public void ResetCombatState()
    {
        if (IsDead || CurrentHP == MaxHP)
            return;

        CurrentHP = MaxHP;
        OnHPChanged?.Invoke(CurrentHP, MaxHP);
    }
}
