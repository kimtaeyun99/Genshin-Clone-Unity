using UnityEngine;

public class CharacterDefenseStat : MonoBehaviour
{
    [SerializeField] private int maxHP;
    [SerializeField] private int currentHP;
    [SerializeField] private int def;

    [SerializeField] private float pyroResistance;
    [SerializeField] private float hydroResistance;
    [SerializeField] private float cryoResistance;
    [SerializeField] private float electroResistance;
    [SerializeField] private float physicalResistance;

    public void SetData(int maxHP, int currentHP, int def, float pyroResistance, float hydroResistance, float cryoResistance,
        float electroResistance, float physicalResistance)
    {
        this.maxHP = maxHP;
        this.currentHP = currentHP;
        this.def = def;

        this.pyroResistance = pyroResistance;
        this.hydroResistance = hydroResistance;
        this.cryoResistance = cryoResistance;
        this.electroResistance = electroResistance;
        this.physicalResistance = physicalResistance;
    }
}
