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
    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public CharacterData GetCharacterData(int index)
    {
        if (index < 0 || index >= partyMembers.Count)
        {
            return null;
        }

        return partyMembers[index];
    }

    public bool SelectCharacter(int index)
    {
        CharacterData data = GetCharacterData(index);

        if (data == null)
        {
            return false;
        }

        if (CharacterManager.Instance == null || !CharacterManager.Instance.SetCurrentCharacter(data))
        {
            return false;
        }
        currentIndex = index;

        OnSelectionChanged?.Invoke();

        return true;
    }

    public bool SelectNextAlive()
    {
        if (CharacterManager.Instance == null)
        {
            return false;
        }

        for (int i = 0; i < partyMembers.Count; i++)
        {
            CharacterRunTime runTime = CharacterManager.Instance.GetCharacter(partyMembers[i]);
            if (runTime == null || runTime.IsDead)
            {
                continue;
            }

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
            {
                continue;
            }

            if (partyMembers.Count >= MaxPartyCount)
            {
                break;
            }

            if (partyMembers.Contains(members[i]))
            {
                continue;
            }

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
