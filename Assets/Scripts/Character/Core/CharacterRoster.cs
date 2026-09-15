using System.Collections.Generic;
using UnityEngine;

public class CharacterRoster : MonoBehaviour
{ 
    [Header("Owned Characters")]
    [SerializeField] private List<CharacterRuntime> ownedCharacters = new();

    [Header("Test")]
    [SerializeField] private List<CharacterData> startingCharacters;
    public IReadOnlyList<CharacterRuntime> OwnedCharacters => ownedCharacters;
    private void Awake()
    {
        foreach (CharacterData characterData in startingCharacters)
        {
            AddCharacter(characterData);
        }
    }
    // 캐릭터 획득
    public bool AddCharacter(CharacterData characterData)
    {
        if (characterData == null)
        {
            return false;
        }

        // 이미 보유한 캐릭터인지 확인
        if (HasCharacter(characterData))
        {
            Debug.LogWarning(
                $"이미 보유 중인 캐릭터입니다: {characterData.CharacterName}"
            );

            return false;
        }

        CharacterRuntime runtime =
            new CharacterRuntime(characterData);

        ownedCharacters.Add(runtime);

        return true;
    }


    // 캐릭터 보유 여부
    public bool HasCharacter(CharacterData characterData)
    {
        if (characterData == null)
        {
            return false;
        }

        foreach (CharacterRuntime runtime in ownedCharacters)
        {
            if (runtime == null)
            {
                continue;
            }

            if (runtime.Data == characterData)
            {
                return true;
            }
        }

        return false;
    }


    // CharacterData로 Runtime 찾기
    public CharacterRuntime GetCharacter(CharacterData characterData)
    {
        if (characterData == null)
        {
            return null;
        }

        foreach (CharacterRuntime runtime in ownedCharacters)
        {
            if (runtime == null)
            {
                continue;
            }

            if (runtime.Data == characterData)
            {
                return runtime;
            }
        }

        return null;
    }
}