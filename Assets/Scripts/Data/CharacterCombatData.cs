using UnityEngine;

[CreateAssetMenu(fileName = "CharacterCombatData", menuName = "GameData/CharacterCombat")]
public class CharacterCombatData : ScriptableObject
{
    [Header("Normal Attack")]
    [SerializeField] private float[] normalAttackMultipliers;

    [Header("Skill")]
    [SerializeField] private float[] skillMultipliers;

    [Header("Burst")]
    [SerializeField] private float[] burstMultipliers;

    public float GetNormalAttackMultipliers(int index)
    {
        return normalAttackMultipliers[index];
    }
    public float GetSkillMultipliers(int index)
    {
        return skillMultipliers[index];
    }

    public float GetBurstMultipliers(int index)
    {
        return burstMultipliers[index];
    }
}
