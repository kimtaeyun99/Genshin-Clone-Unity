using System;
using UnityEngine;

[Serializable]
public class CharacterRuntime
{
    [Header("Data")]
    [SerializeField] private CharacterData data;

    [Header("Level")]
    [SerializeField] private int level;
    [SerializeField] private int ascensionPhase;

    [Header("Current State")]
    [SerializeField] private int currentHP;
    [SerializeField] private float currentEnergy;


    public CharacterData Data => data;

    public int Level => level;
    public int AscensionPhase => ascensionPhase;

    public int CurrentHP => currentHP;
    public float CurrentEnergy => currentEnergy;


    public CharacterRuntime(CharacterData data)
    {
        this.data = data;

        level = 1;
        ascensionPhase = 0;

        currentHP = GetBaseHP();
        currentEnergy = 0f;
    }


    public int GetBaseHP()
    {
        return data.HP + data.HPPerLevel * (level - 1);
    }

    public int GetBaseATK()
    {
        return data.ATK + data.ATKPerLevel * (level - 1);
    }

    public int GetBaseDEF()
    {
        return data.DEF + data.DEFPerLevel * (level - 1);
    }


    public float GetAscensionBonus()
    {
        if (data.AscensionData == null)
        {
            return 0f;
        }

        return data.AscensionData.GetBonusValue(
            ascensionPhase
        );
    }


    public int GetMaxLevel()
    {
        return CharacterLevelRule.GetMaxLevel(
            ascensionPhase
        );
    }


    public bool CanLevelUp()
    {
        return level < GetMaxLevel();
    }


    public bool LevelUp()
    {
        if (!CanLevelUp())
        {
            return false;
        }

        level++;

        return true;
    }


    public bool CanAscend()
    {
        if (ascensionPhase >= 6)
        {
            return false;
        }

        return level >= GetMaxLevel();
    }


    public bool Ascend()
    {
        if (!CanAscend())
        {
            return false;
        }

        ascensionPhase++;

        return true;
    }
}