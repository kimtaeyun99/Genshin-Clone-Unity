using System;
using UnityEngine;
using System.Collections.Generic;

public class CharacterParty : MonoBehaviour
{
    public static CharacterParty Instance { get; private set; }

    private const int MaxPartyCount = 4;

    [Header("Party")]
    [SerializeField] private List<CharacterData> partyMembers = new();

    public event Action OnPartyChanged;
    public event Action OnSelectionChanged;

    private int currentIndex;
    public int CurrentIndex => currentIndex;
    public IReadOnlyList<CharacterData> PartyMembers => partyMembers;
    public int Count => partyMembers.Count;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public bool AddPartyMember(CharacterData data)
    {
        if (data == null || partyMembers.Count >= MaxPartyCount || partyMembers.Contains(data))
            return false;

        partyMembers.Add(data);
        OnPartyChanged?.Invoke();
        return true;
    }

    public bool RemovePartyMember(CharacterData data)
    {
        if (data == null)
            return false;

        // Keep the original return behavior for a non-null argument.
        bool removed = partyMembers.Remove(data);
        if (removed)
            OnPartyChanged?.Invoke();

        return true;
    }

    public bool ChangePartyMember(int index, CharacterData data)
    {
        if (data == null || index < 0 || index >= partyMembers.Count || partyMembers.Contains(data))
            return false;

        partyMembers[index] = data;
        OnPartyChanged?.Invoke();
        return true;
    }

    public CharacterData GetCharacterData(int index)
    {
        if (index < 0 || index >= partyMembers.Count)
            return null;

        return partyMembers[index];
    }

    public bool SelectCharacter(int index)
    {
        CharacterData data = GetCharacterData(index);
        if (data == null)
            return false;

        CharacterManager manager = CharacterManager.Instance;
        if (manager == null || !manager.SetCurrentCharacter(data))
            return false;

        // Unlike CharacterManager's event, this notification fires
        // only AFTER the party's selected index is updated.
        currentIndex = index;
        OnSelectionChanged?.Invoke();
        return true;
    }

    public bool SelectNextAlive()
    {
        CharacterManager manager = CharacterManager.Instance;
        if (manager == null)
            return false;

        for (int i = 0; i < partyMembers.Count; i++)
        {
            CharacterRunTime runTime = manager.GetCharacter(partyMembers[i]);
            if (runTime == null || runTime.IsDead)
                continue;

            return SelectCharacter(i);
        }

        return false;
    }

    public void LoadParty(List<CharacterData> members, int savedCurrentIndex)
    {
        partyMembers.Clear();

        if (members == null)
        {
            currentIndex = 0;
            OnPartyChanged?.Invoke();
            return;
        }

        for (int i = 0; i < members.Count; i++)
        {
            if (members[i] == null)
                continue;

            if (partyMembers.Count >= MaxPartyCount)
                break;

            if (partyMembers.Contains(members[i]))
                continue;

            partyMembers.Add(members[i]);
        }

        if (partyMembers.Count == 0)
        {
            currentIndex = 0;
            OnPartyChanged?.Invoke();
            return;
        }

        currentIndex = Mathf.Clamp(savedCurrentIndex, 0, partyMembers.Count - 1);
        OnPartyChanged?.Invoke();
    }
}
