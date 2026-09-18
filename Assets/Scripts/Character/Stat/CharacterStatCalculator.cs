using UnityEngine;

public class CharacterStatCalculator : MonoBehaviour
{
    [SerializeField] private CharacterAttackStat characterAttackStat;
    [SerializeField] private CharacterDefenseStat characterDefenseStat;

    [SerializeField] private float baseCriticalRate = 5.0f;
    [SerializeField] private float baseCriticalDamage = 50.0f;
    [SerializeField] private float baseEnergyRecharge = 100f;
    [SerializeField] private float baseElementalDamageBonus = 0f;
    [SerializeField] private float baseElementalResistance = 0f;
    public void ApplyStat(CharacterRunTime character)
    {
        CharacterData data = character.Data;

        int level = character.Level;

        int maxHP = data.HP + data.HPPerLevel * (level - 1);
        int atk = data.ATK + data.ATKPerLevel * (level - 1);
        int def = data.DEF + data.DEFPerLevel * (level - 1);

        character.SetMaxHP(maxHP);

        characterAttackStat.SetData(atk, data.ElementalMastery, baseCriticalRate, baseCriticalDamage, 
            baseEnergyRecharge, baseElementalDamageBonus, baseElementalDamageBonus, baseElementalDamageBonus, 
            baseElementalDamageBonus, baseElementalDamageBonus);

        characterDefenseStat.SetData(maxHP, character.CurrentHP, def, 
            baseElementalResistance, baseElementalResistance, baseElementalResistance, 
            baseElementalResistance, baseElementalResistance);
    }
}
