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

    public CharacterData Data => data;
    public CharacterStat Stat => stat;
    public int Level => level;
    public int AscensionPhase => ascensionPhase;
    public int MaxHP => maxHP;
    public int CurrentHP => currentHP;
    public float CurrentEnergy => currentEnergy;
    public float SkillCoolDown => skillCoolDown;
    public float BurstCoolDown => burstCoolDown;

    public CharacterRunTime(CharacterData data)
    {
        this.data = data;

        level = 1;
        ascensionPhase = 0;

        maxHP = data.HP;
        currentHP = 1;

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
        if(damage <= 0)
        {
            return;
        }

        currentHP -= Mathf.RoundToInt(damage);

        if(currentHP < 0)
        {
            currentHP = 0;
        }
    }
}
