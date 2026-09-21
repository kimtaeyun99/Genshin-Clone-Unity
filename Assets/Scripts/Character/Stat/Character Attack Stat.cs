using UnityEngine;

public class CharacterAttackStat : MonoBehaviour
{
    [SerializeField] private int atk;
    [SerializeField] private int elementalMastery;
    [SerializeField] private float criticalRate;
    [SerializeField] private float criticalDamage;

    [SerializeField] private float energyRecharge;

    [SerializeField] private float pyroDamageBonus;
    [SerializeField] private float hydroDamageBonus;
    [SerializeField] private float cryoDamageBonus;
    [SerializeField] private float electroDamageBonus;
    [SerializeField] private float physicalDamageBonus;

    public void SetData(int atk, int elementalMastery, float criticalRate, float criticalDamage, float energyRecharge, 
        float pyroDamageBonus, float hydroDamageBonus, float cryoDamageBonus, float electroDamageBonus, float physicalDamageBonus)
    {
        this.atk = atk;
        this.elementalMastery = elementalMastery;
        this.criticalRate = criticalRate;
        this.criticalDamage = criticalDamage;

        this.energyRecharge = energyRecharge;

        this.pyroDamageBonus = pyroDamageBonus;
        this.hydroDamageBonus = hydroDamageBonus;
        this.cryoDamageBonus = cryoDamageBonus;
        this.electroDamageBonus = electroDamageBonus;
        this.physicalDamageBonus = physicalDamageBonus;
    }
}
