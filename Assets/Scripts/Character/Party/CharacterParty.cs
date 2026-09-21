using UnityEngine;
using System.Collections.Generic;

public class CharacterParty : MonoBehaviour
{
    private const int MaxPartyCount = 4;

    [Header("Party")]
    [SerializeField] private List<CharacterData> partyMembers = new();

    [Header("Manager")]
    [SerializeField] private CharacterManager characterManager;

    private int currentIndex;
    public int CurrentIndex => currentIndex;
    public IReadOnlyList<CharacterData> PartyMembers => partyMembers;
    public int Count => partyMembers.Count;

    public bool AddCharacter(CharacterData data)
    {
        if(data == null)
        {
            return false;
        }
        if(partyMembers.Count >= MaxPartyCount)
        {
            return false;
        }
        if(partyMembers.Contains(data))
        {
            return false;
        }

        partyMembers.Add(data);

        return true;
    }
    public bool RemoveCharacter(CharacterData data)
    {
        if(data == null)
        {
            return false;
        }

        partyMembers.Remove(data);

        return true;
    }
    public bool ChangeCharacter(int index, CharacterData data)
    {
        if(data == null)
        {
            return false;
        }
        if(index < 0 || index >= partyMembers.Count)
        {
            return false;
        }
        if(partyMembers.Contains(data))
        {
            return false;
        }

        partyMembers[index] = data;

        return true;        
    }
    public CharacterData GetCharacterData(int index)
    {
        if(index < 0 || index >= partyMembers.Count)
        {
            return null;
        }

        return partyMembers[index];
    }
    public bool SelectCharacter(int index)
    {
        CharacterData data = GetCharacterData(index);

        if(data == null)
        {
            return false;
        }

        if(!characterManager.SetCurrentCharacter(data))
        {
            return false;
        }

        currentIndex = index;

        return true;
    }
}
