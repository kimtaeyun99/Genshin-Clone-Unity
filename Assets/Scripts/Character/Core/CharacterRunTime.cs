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

        if(level >= maxLevel)
        {
            return false;
        }

        level++;
        return true;

    }
    public bool Ascension()
    {
        if(ascensionPhase >= CharacterLevelRule.MaxLevels.Length - 1)
        {
            return false;
        }

        int maxLevel = CharacterLevelRule.GetMaxLevel(ascensionPhase);

        if(level < maxLevel)
        {
            return false;
        }

        ascensionPhase++;

        return true;
    }
    public void SetMaxHP(int newMaxHP)
    {
        int hpShift = newMaxHP - maxHP;

        maxHP = newMaxHP;
        currentHP += hpShift;

        if(currentHP > maxHP)
        {
            currentHP = maxHP;
        }
    }
    public void Heal(int amount)
    {
        if (IsDead)
        {
            return;
        }
        if (amount <= 0)
        {
            return;
        }

        currentHP += amount;

        if(currentHP > maxHP)
        {
            currentHP = maxHP;
        }
    }
    public void TakeDamage(float damage)
    {
        if(IsDead || damage <= 0)
        {
            return;
        }

        currentHP -= Mathf.RoundToInt(damage);

        if(currentHP <= 0)
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
        skillCoolDown = coolDown;
    }

    public void StartBurstCoolDown(float coolDown)
    {
        burstCoolDown = coolDown;
    }

    public void UpdateCoolDown(float deltaTime)
    {
        if (skillCoolDown > 0f)
        {
            skillCoolDown -= deltaTime;

            if (skillCoolDown < 0f)
                skillCoolDown = 0f;
        }

        if (burstCoolDown > 0f)
        {
            burstCoolDown -= deltaTime;

            if (burstCoolDown < 0f)
                burstCoolDown = 0f;
        }
    }
    public void AddEnergy(float amount)
    {
        if (amount <= 0f)
        {
            return;
        }

        currentEnergy += amount;

        if (currentEnergy > MaxEnergy)
        {
            currentEnergy = MaxEnergy;
        }
    }

    public bool ConsumeBurstEnergy()
    {
        if (currentEnergy < MaxEnergy)
        {
            return false;
        }

        currentEnergy = 0f;
        return true;
    }
    public void Respawn()
    {
        IsDead = false;
        currentHP = maxHP;
        currentEnergy = 0f;

        skillCoolDown = 0f;
        burstCoolDown = 0f;
    }
    public void LoadGrowthData(
        int level,
        int ascensionPhase)
    {
        this.level = level;
        this.ascensionPhase = ascensionPhase;
    }

    public void LoadStateData(
        int currentHP,
        float currentEnergy,
        bool isDead)
    {
        this.currentHP = Mathf.Clamp(
            currentHP,
            0,
            maxHP
        );

        this.currentEnergy = Mathf.Clamp(
            currentEnergy,
            0f,
            MaxEnergy
        );

        IsDead = isDead;
    }
}
