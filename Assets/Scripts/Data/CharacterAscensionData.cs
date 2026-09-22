using UnityEngine;

[CreateAssetMenu(
    fileName = "CharacterAscensionData",
    menuName = "GameData/Character Ascension"
)]
public class CharacterAscensionData : ScriptableObject
{
    [Header("Character")]
    [SerializeField] private string characterId;
    [SerializeField] private string characterName;

    [Header("Ascension Bonus")]
    [SerializeField] private StatType ascensionStatType;
    [SerializeField] private float[] bonusValues = new float[7];

    public string CharacterId => characterId;
    public string CharacterName => characterName;
    public StatType AscensionStatType => ascensionStatType;
    public float GetBonusValue(int ascensionPhase)
    {
        if (ascensionPhase < 0 || ascensionPhase >= bonusValues.Length)
        {
            return 0f;
        }

        return bonusValues[ascensionPhase];
    }
    public void SetData(string characterId,string characterName,StatType ascensionStatType, float[] bonusValues)
    {
        this.characterId = characterId;
        this.characterName = characterName;
        this.ascensionStatType = ascensionStatType;
        this.bonusValues = bonusValues;
    }
}