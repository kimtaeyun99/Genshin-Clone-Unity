using System;
using UnityEngine;

[Serializable]
public class CharacterRunTime
{
    private CharacterData data;
    private CharacterStat stat;

    private int level;
    private int ascensionPhase;
    private int maxHP;
    private int currentHP;

    private float currentEnergy;

    private float skillCoolDown;
    private float burstCoolDown;

    // CombatUI가 매 프레임 확인하지 않아도 상태 변화를 알 수 있도록 알림 제공.
    public event Action OnEnergyChanged;
    public event Action OnSkillCooldownChanged;
    public event Action OnBurstCooldownChanged;

    public float CurrentEnergy => currentEnergy;
    public float MaxEnergy => data.MaxEnergy;

    public float SkillCoolDown => skillCoolDown;
    public float BurstCoolDown => burstCoolDown;

    public bool IsSkillReady => skillCoolDown <= 0f;
    public bool IsBurstReady => burstCoolDown <= 0f && currentEnergy >= MaxEnergy;

    public CharacterData Data => data;
    public CharacterStat Stat => stat;
    public int Level => level;
    public int AscensionPhase => ascensionPhase;
    public int MaxHP => maxHP;
    public int CurrentHP => currentHP;

    public bool IsDead;

    public CharacterRunTime(CharacterData data)
    {
        this.data = data;

        level = 1;
        ascensionPhase = 0;

        maxHP = data.HP;
        currentHP = maxHP;

        currentEnergy = 0f;
        skillCoolDown = 0f;
        burstCoolDown = 0f;
    }

    public void SetStat(CharacterStat stat)
    {
        this.stat = stat;
    }

    public bool LevelUp()
    {
        int maxLevel = CharacterLevelRule.GetMaxLevel(ascensionPhase);
        if (level >= maxLevel)
            return false;

        level++;
        return true;
    }

    public bool Ascension()
    {
        if (ascensionPhase >= CharacterLevelRule.MaxLevels.Length - 1)
            return false;

        int maxLevel = CharacterLevelRule.GetMaxLevel(ascensionPhase);
        if (level < maxLevel)
            return false;

        ascensionPhase++;
        return true;
    }

    public void SetMaxHP(int newMaxHP)
    {
        int hpShift = newMaxHP - maxHP;
        maxHP = newMaxHP;
        currentHP += hpShift;

        if (currentHP > maxHP)
            currentHP = maxHP;
    }

    public void Heal(int amount)
    {
        if (IsDead || amount <= 0)
            return;

        currentHP += amount;
        if (currentHP > maxHP)
            currentHP = maxHP;
    }

    public void TakeDamage(float damage)
    {
        if (IsDead || damage <= 0)
            return;

        currentHP -= Mathf.RoundToInt(damage);
        if (currentHP <= 0)
        {
            currentHP = 0;
            IsDead = true;
        }
    }

    public void Revive()
    {
        IsDead = false;
    }

    public void StartSkillCoolDown(float coolDown)
    {
        if (skillCoolDown == coolDown)
            return;

        skillCoolDown = coolDown;
        OnSkillCooldownChanged?.Invoke();
    }

    public void StartBurstCoolDown(float coolDown)
    {
        if (burstCoolDown == coolDown)
            return;

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
                skillCoolDown = 0f;

            if (skillCoolDown != oldCooldown)
                OnSkillCooldownChanged?.Invoke();
        }

        if (burstCoolDown > 0f)
        {
            float oldCooldown = burstCoolDown;
            burstCoolDown -= deltaTime;
            if (burstCoolDown < 0f)
                burstCoolDown = 0f;

            if (burstCoolDown != oldCooldown)
                OnBurstCooldownChanged?.Invoke();
        }
    }

    public void AddEnergy(float amount)
    {
        if (amount <= 0f)
            return;

        float newEnergy = Mathf.Min(currentEnergy + amount, MaxEnergy);
        if (newEnergy == currentEnergy)
            return;

        currentEnergy = newEnergy;
        OnEnergyChanged?.Invoke();
    }

    public bool ConsumeBurstEnergy()
    {
        if (currentEnergy < MaxEnergy)
            return false;

        bool energyChanged = currentEnergy != 0f;
        currentEnergy = 0f;
        if (energyChanged)
            OnEnergyChanged?.Invoke();
        return true;
    }

    public void Respawn()
    {
        bool energyChanged = currentEnergy != 0f;
        bool skillChanged = skillCoolDown != 0f;
        bool burstChanged = burstCoolDown != 0f;

        IsDead = false;
        currentHP = maxHP;
        currentEnergy = 0f;
        skillCoolDown = 0f;
        burstCoolDown = 0f;

        if (energyChanged) OnEnergyChanged?.Invoke();
        if (skillChanged) OnSkillCooldownChanged?.Invoke();
        if (burstChanged) OnBurstCooldownChanged?.Invoke();
    }

    public void LoadGrowthData(int level, int ascensionPhase)
    {
        this.level = level;
        this.ascensionPhase = ascensionPhase;
    }

    public void LoadStateData(int currentHP, float currentEnergy, bool isDead)
    {
        this.currentHP = Mathf.Clamp(currentHP, 0, maxHP);

        float newEnergy = Mathf.Clamp(currentEnergy, 0f, MaxEnergy);
        bool energyChanged = this.currentEnergy != newEnergy;
        this.currentEnergy = newEnergy;

        IsDead = isDead;

        if (energyChanged)
            OnEnergyChanged?.Invoke();
    }
}
