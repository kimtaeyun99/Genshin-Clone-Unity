using UnityEngine;
public class CharacterStatCalculator
{
    private float baseCriticalRate = 5.0f;
    private float baseCriticalDamage = 50.0f;
    private float baseEnergyRecharge = 100f;
    private float baseElementalDamageBonus = 0f;
    private float baseElementalResistance = 0f;

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

    public CharacterStat ApplyStat(CharacterRunTime character)
    {
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

        CharacterData data = character.Data;

        level = character.Level;

        maxHP = data.HP + data.HPPerLevel * (level - 1);
        atk = data.ATK + data.ATKPerLevel * (level - 1);
        def = data.DEF + data.DEFPerLevel * (level - 1);
        elementalMastery = data.ElementalMastery;

        CharacterAscensionData characterAscensionData = data.AscensionData;

        if (characterAscensionData != null)
        {
            StatType ascensionStatType = characterAscensionData.AscensionStatType;
            float bonusValue = characterAscensionData.GetBonusValue(character.AscensionPhase);

            switch (ascensionStatType)
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

        character.SetMaxHP(maxHP);

        CharacterStat stat = new CharacterStat();

        stat.SetData(
            atk,
            elementalMastery,
            criticalRate,
            criticalDamage,
            energyRecharge,
            pyroDamageBonus,
            hydroDamageBonus,
            cryoDamageBonus,
            electroDamageBonus,
            physicalDamageBonus,
            maxHP,
            character.CurrentHP,
            def,
            pyroResistance,
            hydroResistance,
            cryoResistance,
            electroResistance,
            physicalResistance
        );

        return stat;
    }
    private int CalculatePercent(int value, float percent)
    {
        return Mathf.RoundToInt(value * (1f + percent / 100));
    }
}
