public class CharacterStat
{
    // Attack
    public int ATK { get; private set; }
    public int ElementalMastery { get; private set; }
    public float CriticalRate { get; private set; }
    public float CriticalDamage { get; private set; }
    public float EnergyRecharge { get; private set; }

    public float PyroDamageBonus { get; private set; }
    public float HydroDamageBonus { get; private set; }
    public float CryoDamageBonus { get; private set; }
    public float ElectroDamageBonus { get; private set; }
    public float PhysicalDamageBonus { get; private set; }

    // Defense
    public int MaxHP { get; private set; }
    public int CurrentHP { get; private set; }
    public int DEF { get; private set; }

    public float PyroResistance { get; private set; }
    public float HydroResistance { get; private set; }
    public float CryoResistance { get; private set; }
    public float ElectroResistance { get; private set; }
    public float PhysicalResistance { get; private set; }

    public void SetData(
        int atk,
        int elementalMastery,
        float criticalRate,
        float criticalDamage,
        float energyRecharge,
        float pyroDamageBonus,
        float hydroDamageBonus,
        float cryoDamageBonus,
        float electroDamageBonus,
        float physicalDamageBonus,
        int maxHP,
        int currentHP,
        int def,
        float pyroResistance,
        float hydroResistance,
        float cryoResistance,
        float electroResistance,
        float physicalResistance)
    {
        ATK = atk;
        ElementalMastery = elementalMastery;
        CriticalRate = criticalRate;
        CriticalDamage = criticalDamage;
        EnergyRecharge = energyRecharge;

        PyroDamageBonus = pyroDamageBonus;
        HydroDamageBonus = hydroDamageBonus;
        CryoDamageBonus = cryoDamageBonus;
        ElectroDamageBonus = electroDamageBonus;
        PhysicalDamageBonus = physicalDamageBonus;

        MaxHP = maxHP;
        CurrentHP = currentHP;
        DEF = def;

        PyroResistance = pyroResistance;
        HydroResistance = hydroResistance;
        CryoResistance = cryoResistance;
        ElectroResistance = electroResistance;
        PhysicalResistance = physicalResistance;
    }
}