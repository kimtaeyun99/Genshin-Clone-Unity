using System;
using UnityEngine;

[Serializable]
public class CharacterRunTime
{
    private CharacterData data;

    private int atk;
    private int maxHP;
    private int currentHP;

    private int currentEnergy;
    private int burstEnergyCost;
    private float skillCoolDown;
    private float burstCoolDown;
    
    public CharacterData Data => data;
    public int ATK => atk;
    public int MaxHP => maxHP;
    public int CurrentHP => currentHP;
    public int CurrentEnergy => currentEnergy;
    public int BurstEnergyCost => burstEnergyCost;
    public float SkillCooldown => skillCoolDown;
    public float BurstCooldown => burstCoolDown;
    
    public event Action OnEnergyChanged;
    public event Action OnSkillCooldownChanged;
    public event Action OnBurstCooldownChanged;
    public event Action<int, int> OnHealthChanged;

    public bool IsSkillReady => skillCoolDown <= 0f;
    public bool IsBurstReady => burstCoolDown <= 0f && currentEnergy >= burstEnergyCost;
    public bool IsDead { get; private set; }

    public CharacterRunTime(CharacterData data)
    {
        this.data = data;
        this.atk = data.ATK;
        this.maxHP = data.MaxHP;
        this.currentHP = maxHP;
        this.currentEnergy = 0;
        this.burstEnergyCost = data.BurstEnergyCost;
        this.skillCoolDown = 0f;
        this.burstCoolDown = 0f;
    }
    public void Heal(int amount)
    {
        if (IsDead || amount <= 0)
        {
            return;
        }

        int previousHP = currentHP;
        currentHP += amount;
        if (currentHP > maxHP)
        {
            currentHP = maxHP;
        }

        if(currentHP != maxHP)
        {
            OnHealthChanged?.Invoke(currentHP, maxHP);
        }
    }

    public void TakeDamage(float damage)
    {
        if (IsDead || damage <= 0f)
        {
            return;
        }

        currentHP -= Mathf.RoundToInt(damage);
        if (currentHP <= 0)
        {
            currentHP = 0;
            IsDead = true;
        }

        OnHealthChanged?.Invoke(currentHP, maxHP);
    }

    public void StartSkillCoolDown(float coolDown)
    {
        if (skillCoolDown == coolDown)
        {
            return;
        }

        skillCoolDown = coolDown;
        OnSkillCooldownChanged?.Invoke();
    }

    public void StartBurstCoolDown(float coolDown)
    {
        if (burstCoolDown == coolDown)
        {
            return;
        }

        burstCoolDown = coolDown;
        OnBurstCooldownChanged?.Invoke();
    }

    public void UpdateCoolDown(float deltaTime)
    {
        if (skillCoolDown > 0f)
        {
            float oldCooldown = skillCoolDown;
            skillCoolDown -= deltaTime;
            if (skillCoolDown < 0f)
            {
                skillCoolDown = 0f;
            }

            if (skillCoolDown != oldCooldown)
            {
                OnSkillCooldownChanged?.Invoke();
            }
        }

        if (burstCoolDown > 0f)
        {
            float oldCooldown = burstCoolDown;
            burstCoolDown -= deltaTime;
            if (burstCoolDown < 0f)
            {
                burstCoolDown = 0f;
            }

            if (burstCoolDown != oldCooldown)
            {
                OnBurstCooldownChanged?.Invoke();
            }
        }
    }

    public void AddEnergy(int amount)
    {
        if (amount <= 0f)
        {
            return;
        }

        currentEnergy += amount;
        if(currentEnergy > burstEnergyCost)
        {
            currentEnergy = burstEnergyCost;
        }
       
        OnEnergyChanged?.Invoke();
    }

    public bool ConsumeBurstEnergy()
    {
        if (currentEnergy < burstEnergyCost)
        {
            return false;
        }

        currentEnergy = 0;
        OnEnergyChanged?.Invoke();
        return true;
    }

    public void Respawn()
    {
        IsDead = false;
        currentHP = maxHP;
        currentEnergy = 0;
        skillCoolDown = 0f;
        burstCoolDown = 0f;

        OnHealthChanged?.Invoke(currentHP, maxHP);
        OnEnergyChanged?.Invoke();
        OnSkillCooldownChanged?.Invoke();
        OnBurstCooldownChanged?.Invoke();
    }

    public void LoadStateData(int currentHP, int currentEnergy, bool isDead)
    {
        this.currentHP = Mathf.Clamp(currentHP, 0, maxHP);
        this.currentEnergy = Mathf.Clamp(currentEnergy, 0, burstEnergyCost);
        IsDead = isDead;

        OnHealthChanged?.Invoke(this.currentHP, maxHP);
        OnEnergyChanged?.Invoke();
    }
}
