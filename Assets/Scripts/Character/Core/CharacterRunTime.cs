using System;
using UnityEngine;

[Serializable]
public class CharacterRunTime
{
    public CharacterData Data { get; private set; }
    public int ATK { get; private set; }
    public int MaxHP { get; private set; }
    public int CurrentHP { get; private set; }
    public int CurrentEnergy { get; private set; }
    public int BurstEnergyCost { get; private set; }
    public float SkillCoolDown { get; private set; }
    public float BurstCoolDown { get; private set; }
    
    public event Action OnEnergyChanged;
    public event Action OnSkillCoolDownChanged;
    public event Action OnBurstCoolDownChanged;
    public event Action<int, int> OnHealthChanged;

    public bool IsSkillReady => SkillCoolDown <= 0f;
    public bool IsBurstReady => BurstCoolDown <= 0f && CurrentEnergy >= BurstEnergyCost;
    public bool IsDead { get; private set; }

    public CharacterRunTime(CharacterData data)
    {
        Data = data;
        ATK = data.ATK;
        MaxHP = data.MaxHP;
        CurrentHP = MaxHP;
        CurrentEnergy = 0;
        BurstEnergyCost = data.BurstEnergyCost;
        SkillCoolDown = 0;
        BurstCoolDown = 0;
    }
    public void Heal(int amount)
    {
        if (IsDead || amount <= 0)
        {
            return;
        }

        int previousHP = CurrentHP;
        CurrentHP += amount;
        if (CurrentHP > MaxHP)
        {
            CurrentHP = MaxHP;
        }

        if(CurrentHP != MaxHP)
        {
            OnHealthChanged?.Invoke(CurrentHP, MaxHP);
        }
    }

    public void TakeDamage(float damage)
    {
        if (IsDead || damage <= 0f)
        {
            return;
        }

        CurrentHP -= Mathf.RoundToInt(damage);
        if (CurrentHP <= 0)
        {
            CurrentHP = 0;
            IsDead = true;
        }

        OnHealthChanged?.Invoke(CurrentHP, MaxHP);
    }

    public void StartSkillCoolDown(float coolDown)
    {
        if (SkillCoolDown == coolDown)
        {
            return;
        }

        SkillCoolDown = coolDown;
        OnSkillCoolDownChanged?.Invoke();
    }

    public void StartBurstCoolDown(float coolDown)
    {
        if (BurstCoolDown == coolDown)
        {
            return;
        }

        BurstCoolDown = coolDown;
        OnBurstCoolDownChanged?.Invoke();
    }

    public void UpdateCoolDown(float deltaTime)
    {
        if (SkillCoolDown > 0f)
        {
            float oldCooldown = SkillCoolDown;
            SkillCoolDown -= deltaTime;
            if (SkillCoolDown < 0f)
            {
                SkillCoolDown = 0f;
            }

            if (SkillCoolDown != oldCooldown)
            {
                OnSkillCoolDownChanged?.Invoke();
            }
        }

        if (BurstCoolDown > 0f)
        {
            float oldCooldown = BurstCoolDown;
            BurstCoolDown -= deltaTime;
            if (BurstCoolDown < 0f)
            {
                BurstCoolDown = 0f;
            }

            if (BurstCoolDown != oldCooldown)
            {
                OnBurstCoolDownChanged?.Invoke();
            }
        }
    }

    public void AddEnergy(int amount)
    {
        if (amount <= 0f)
        {
            return;
        }

        CurrentEnergy += amount;
        if(CurrentEnergy > BurstEnergyCost)
        {
            CurrentEnergy = BurstEnergyCost;
        }
       
        OnEnergyChanged?.Invoke();
    }

    public bool ConsumeBurstEnergy()
    {
        if (CurrentEnergy < BurstEnergyCost)
        {
            return false;
        }

        CurrentEnergy = 0;
        OnEnergyChanged?.Invoke();
        return true;
    }

    public void Respawn()
    {
        IsDead = false;
        CurrentHP = MaxHP;
        CurrentEnergy = 0;
        SkillCoolDown = 0f;
        BurstCoolDown = 0f;

        OnHealthChanged?.Invoke(CurrentHP, MaxHP);
        OnEnergyChanged?.Invoke();
        OnSkillCoolDownChanged?.Invoke();
        OnBurstCoolDownChanged?.Invoke();
    }

    public void LoadStateData(int CurrentHP, int CurrentEnergy, bool isDead)
    {
        this.CurrentHP = Mathf.Clamp(CurrentHP, 0, MaxHP);
        this.CurrentEnergy = Mathf.Clamp(CurrentEnergy, 0, BurstEnergyCost);
        IsDead = isDead;

        OnHealthChanged?.Invoke(this.CurrentHP, MaxHP);
        OnEnergyChanged?.Invoke();
    }
}
