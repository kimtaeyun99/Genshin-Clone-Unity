using UnityEngine;
using System.Collections.Generic;

public class CharacterParty : MonoBehaviour
{
    public static CharacterParty Instance { get; private set; }

    private const int MaxPartyCount = 4;

    [Header("Party")]
    [SerializeField] private List<CharacterData> partyMembers = new();

    [Header("Manager")]
    [SerializeField] private CharacterManager characterManager;

    private int currentIndex;
    public int CurrentIndex => currentIndex;
    public IReadOnlyList<CharacterData> PartyMembers => partyMembers;
    public int Count => partyMembers.Count;
    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(Instance);
            return;
        }

        Instance = this;
    }
    public bool AddPartyMember(CharacterData data)
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
    public bool RemovePartyMember(CharacterData data)
    {
        if(data == null)
        {
            return false;
        }

        partyMembers.Remove(data);

        return true;
    }
    public bool ChangePartyMember(int index, CharacterData data)
    {
        if (data == null)
        {
            return false;
        }
        if (index < 0 || index >= partyMembers.Count)
        {
            return false;
        }
        if (partyMembers.Contains(data))
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
    public bool SelectNextAlive()
    {
        for (int i=0; i< partyMembers.Count; i++)
        {
            CharacterRunTime runTime = characterManager.GetCharacter(partyMembers[i]);
            if(runTime == null || runTime.IsDead)
            {
                continue;
            }
            return SelectCharacter(i);
        }
        return false;
    }
}
