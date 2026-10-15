using System;
using UnityEngine;

public sealed class MonsterRunTime
{
    public MonsterData data { get; private set; }
    public int ATK { get; private set; }
    public float MaxHP { get; private set; }
    public float CurrentHP { get; private set; }
    public bool IsDead { get; private set; }

    public event Action<float, float> OnHPChanged;
    public event Action OnDead;

    public MonsterRunTime(MonsterData data)
    {
        this.data = data;
        ATK = data.ATK;
        MaxHP = data.MaxHP;
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
