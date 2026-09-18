using UnityEngine;

[CreateAssetMenu(
    fileName = "CharacterAscensionData",
    menuName = "GameData/Character Ascension"
)]
public class CharacterAscensionData : ScriptableObject
{
    [SerializeField] private string characterId;

    [Header("Ascension Bonus")]
    [SerializeField] private StatType ascensionStatType;
    [SerializeField] private float[] bonusValues = new float[7];

    public string CharacterId => characterId;
    public StatType AscensionStatType => ascensionStatType;
    public float GetBonusValue(int ascensionPhase)
    {
        if (ascensionPhase < 0 || ascensionPhase >= bonusValues.Length)
        {
            return 0f;
        }

        return bonusValues[ascensionPhase];
    }
    public void SetData(string characterId,StatType ascensionStatType, float[] bonusValues)
    {
        this.characterId = characterId;
        this.ascensionStatType = ascensionStatType;
        this.bonusValues = bonusValues;
    }
}