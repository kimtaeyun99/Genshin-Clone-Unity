using System;
using UnityEngine;

[Serializable]
public class CharacterRuntime
{
    [Header("Data")]
    [SerializeField] private CharacterData data;
    [SerializeField] private CharacterAscensionData ascensionData;

    [Header("Level")]
    [SerializeField] private int level;
    [SerializeField] private int ascensionPhase;

    [Header("Current State")]
    [SerializeField] private int currentHP;
    [SerializeField] private float currentEnergy;


    // Data
    public CharacterData Data => data;
    public CharacterAscensionData AscensionData => ascensionData;

    // Level
    public int Level => level;
    public int AscensionPhase => ascensionPhase;

    // Current State
    public int CurrentHP => currentHP;
    public float CurrentEnergy => currentEnergy;


    public CharacterRuntime(
        CharacterData data,
        CharacterAscensionData ascensionData)
    {
        this.data = data;
        this.ascensionData = ascensionData;

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


    public int GetMaxLevel()
    {
        return CharacterLevelRule.GetMaxLevel(ascensionPhase);
    }

    public bool CanLevelUp()
    {
        return level < GetMaxLevel();
    }

    // 레벨업
    public bool LevelUp()
    {
        if (!CanLevelUp())
        {
            return false;
        }

        level++;

        return true;
    }

    // 돌파 가능한지 확인
    public bool CanAscend()
    {
        // 최종 돌파 단계
        if (ascensionPhase >= 6)
        {
            return false;
        }

        // 현재 돌파 단계의 최대 레벨에 도달해야 돌파 가능
        if (level < GetMaxLevel())
        {
            return false;
        }

        return true;
    }

    // 돌파
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