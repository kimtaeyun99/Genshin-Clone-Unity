using UnityEngine;
using System.Collections.Generic;

public class CharacterManager : MonoBehaviour
{
    [Header("Character Data")]
    [SerializeField] private List<CharacterData> characterDatas;

    [Header("StatCalculator")]
    [SerializeField] private CharacterStatCalculator statCalculator;

    [Header("View")]
    [SerializeField] private CharacterView characterView;

    private CharacterRunTime currentCharacter;

    public CharacterRunTime CurrentCharacter => currentCharacter;

    private Dictionary<CharacterData, CharacterRunTime> characterRunTimes;

    private void Awake()
    {
        InitializeCharacters();
    }
    private void InitializeCharacters()
    {
        characterRunTimes = new Dictionary<CharacterData, CharacterRunTime>();

        foreach(CharacterData data in characterDatas)
        {
            if (data == null)
            {
                continue;
            }

            if(characterRunTimes.ContainsKey(data))
            {
                continue;
            }

            CharacterRunTime runTime = new CharacterRunTime(data);

            characterRunTimes.Add(data, runTime);
        }
    }
    public CharacterRunTime GetCharacter(CharacterData data)
    {
        if(characterRunTimes.TryGetValue(data, out CharacterRunTime runTime))
        {
            return runTime;
        }

        return null;
    }
    public bool SetCurrentCharacter(CharacterData data)
    {
        if (!characterRunTimes.TryGetValue(data, out CharacterRunTime runTime))
        {
            return false;
        }

        currentCharacter = runTime;

        statCalculator.ApplyStat(currentCharacter);

        characterView.ChangeView(data);

        return true;
    }
}
