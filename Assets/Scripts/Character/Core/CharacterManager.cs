using System;
using UnityEngine;
using System.Collections.Generic;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager Instance { get; private set; }

    [Header("Character Data")]
    [SerializeField] private List<CharacterData> characterDatas;

    [Header("Effect Root")]
    [SerializeField] private Transform effectRoot;

    [Header("View")]
    [SerializeField] private CharacterView characterView;

    private Dictionary<CharacterData, CharacterRunTime> characterRunTimes = new Dictionary<CharacterData, CharacterRunTime>();

    private CharacterRunTime currentCharacter;

    public List<CharacterData> CharacterDatas => characterDatas;
    public Transform EffectRoot => effectRoot;
    public Animator CurrentAnimator => characterView.CurrentAnimator;
    public CharacterRunTime CurrentCharacter => currentCharacter;
    public CharacterView CharacterView => characterView;
    public ICharacterAttack CurrentAttack => characterView.CurrentAttack;
    public ICharacterSkill CurrentSkill => characterView.CurrentSkill;
    public ICharacterBurst CurrentBurst => characterView.CurrentBurst;

    public event Action<CharacterRunTime> OnCurrentCharacterChanged;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        InitializeCharacters();
    }

    private void Update()
    {
        UpdateCharacterCoolDown();
    }

    private void InitializeCharacters()
    {
        foreach (CharacterData data in characterDatas)
        {
            if (data == null)
            {
                continue;
            }

            if (characterRunTimes.ContainsKey(data))
            {
                continue;
            }

            CharacterRunTime runTime = new CharacterRunTime(data);

            characterRunTimes.Add(data, runTime);
        }
    }

    public CharacterRunTime GetCharacter(CharacterData data)
    {
        if (characterRunTimes.TryGetValue(data, out CharacterRunTime runTime))
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

        if (runTime.IsDead)
        {
            return false;
        }

        currentCharacter = runTime;

        characterView.ChangeView(data);

        OnCurrentCharacterChanged?.Invoke(currentCharacter);
        return true;
    }

    private void UpdateCharacterCoolDown()
    {
        foreach (CharacterRunTime runTime in characterRunTimes.Values)
        {
            runTime.UpdateCoolDown(Time.deltaTime);
        }
    }
    public void RespawnParty()
    {
        foreach (CharacterData partyMember in CharacterParty.Instance.PartyMembers)
        {
            CharacterRunTime runTime = GetCharacter(partyMember);

            if (runTime == null)
            {
                continue;
            }

            runTime.Respawn();
        }

        if (CharacterParty.Instance.PartyMembers.Count > 0)
        {
            SetCurrentCharacter(CharacterParty.Instance.PartyMembers[0]);
        }
    }

    public CharacterData GetCharacterData(string characterId)
    {
        foreach (CharacterData data in characterDatas)
        {
            if (data == null)
            {
                continue;
            }

            if (data.ID == characterId)
            {
                return data;
            }
        }
        return null;
    }
}
