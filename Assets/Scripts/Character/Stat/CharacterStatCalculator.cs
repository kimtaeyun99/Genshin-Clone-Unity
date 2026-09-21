using System.Collections;
using UnityEngine;

public class CharacterStatCalculator : MonoBehaviour
{
    [Header("Stat")]
    [SerializeField] private CharacterAttackStat characterAttackStat;
    [SerializeField] private CharacterDefenseStat characterDefenseStat;

    [Header("Base Stat")]
    [SerializeField] private float baseCriticalRate = 5.0f;
    [SerializeField] private float baseCriticalDamage = 50.0f;
    [SerializeField] private float baseEnergyRecharge = 100f;
    [SerializeField] private float baseElementalDamageBonus = 0f;
    [SerializeField] private float baseElementalResistance = 0f;

    private int level;

    private int atk;
    private int elementalMastery;
    private float criticalRate;
    private float criticalDamage;
    
    private float energyRecharge;

    private float pyroDamageBonus;
    private float hydroDamageBonus;
    private float cryoDamageBonus;
    private float electroDamageBonus;
    private float physicalDamageBonus;

    private int maxHP;
    private int def;

    private float pyroResistance;
    private float hydroResistance;
    private float cryoResistance;
    private float electroResistance;
    private float physicalResistance;

    public void ApplyStat(CharacterRunTime character)
    {
        //기본 스탯값 적용
        criticalRate = baseCriticalRate;
        criticalDamage = baseCriticalDamage;
        energyRecharge = baseEnergyRecharge;

        pyroDamageBonus = baseElementalDamageBonus;
        hydroDamageBonus = baseElementalDamageBonus;
        cryoDamageBonus = baseElementalDamageBonus;
        electroDamageBonus = baseElementalDamageBonus;
        physicalDamageBonus = baseElementalDamageBonus;

        pyroResistance = baseElementalResistance;
        hydroResistance = baseElementalResistance;
        cryoResistance = baseElementalResistance;
        electroResistance = baseElementalResistance;
        physicalResistance = baseElementalResistance;

        //데이터 적용
        CharacterData data = character.Data;

        //레벨 스탯
        level = character.Level;

        maxHP = data.HP + data.HPPerLevel * (level - 1);
        atk = data.ATK + data.ATKPerLevel * (level - 1);
        def = data.DEF + data.DEFPerLevel * (level - 1);
        elementalMastery = data.ElementalMastery;

        //돌파 스탯
        CharacterAscensionData characterAscensionData = data.AscensionData;
        if (characterAscensionData != null)
        {
            StatType ascensionStatType = characterAscensionData.AscensionStatType;
            float bonusValue = characterAscensionData.GetBonusValue(character.AscensionPhase);

            switch(ascensionStatType)
            {
                case StatType.CritRate:
                    criticalRate += bonusValue;
                    break;
                case StatType.CritDamage:
                    criticalDamage += bonusValue;
                    break;
                case StatType.EnergyRecharge:
                    energyRecharge += bonusValue;
                    break;
                case StatType.ElementalMastery:
                    elementalMastery += (int)bonusValue;
                    break;
                case StatType.HPPercent:
                    maxHP = CalculatePercent(maxHP, bonusValue);
                    break;
            }
        }

        //최종 적용
        character.SetMaxHP(maxHP);

        characterAttackStat.SetData(atk, elementalMastery, criticalRate, criticalDamage, energyRecharge,
            pyroDamageBonus, hydroDamageBonus, cryoDamageBonus, electroDamageBonus, physicalDamageBonus);

        characterDefenseStat.SetData(maxHP,character.CurrentHP,def,pyroResistance,hydroResistance,cryoResistance,
            electroResistance,physicalResistance);
    }
    private int CalculatePercent(int value, float percent)
    {
        return Mathf.RoundToInt(value * (1f + percent / 100));
    }
}
