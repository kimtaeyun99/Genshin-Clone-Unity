using System;
using UnityEngine;
using System.Collections.Generic;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager Instance { get; private set; }

    // CurrentCharacter 변경은 CombatUI 등 여러 UI에서 구독 가능.
    public event Action<CharacterRunTime> OnCurrentCharacterChanged;

    [Header("Character Data")]
    [SerializeField] private List<CharacterData> characterDatas;

    [Header("View")]
    [SerializeField] private CharacterView characterView;

    [Header("Animator")]
    public Animator CurrentAnimator => characterView.CurrentAnimator;
    [Header("AttackHitbox")]
    public AttackHitbox CurrentAttackHitbox => characterView.CurrentAttackHitbox;
    [Header("Effect Root")]
    [SerializeField] private Transform effectRoot;
    public Transform EffectRoot => effectRoot;

    private CharacterRunTime currentCharacter;
    private CharacterStatCalculator statCalculator;
    public CharacterRunTime CurrentCharacter => currentCharacter;

    private Dictionary<CharacterData, CharacterRunTime> characterRunTimes;

    public CharacterView CharacterView => characterView;
    public ICharacterAttack CurrentAttack => characterView.CurrentAttack;
    public ICharacterSkill CurrentSkill => characterView.CurrentSkill;
    public ICharacterBurst CurrentBurst => characterView.CurrentBurst;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        statCalculator = new CharacterStatCalculator();
        InitializeCharacters();
    }

    private void Update()
    {
        UpdateCharacterCoolDown();
    }

    private void InitializeCharacters()
    {
        characterRunTimes = new Dictionary<CharacterData, CharacterRunTime>();

        foreach (CharacterData data in characterDatas)
        {
            if (data == null)
                continue;

            if (characterRunTimes.ContainsKey(data))
                continue;

            CharacterRunTime runTime = new CharacterRunTime(data);
            characterRunTimes.Add(data, runTime);
        }
    }

    public CharacterRunTime GetCharacter(CharacterData data)
    {
        if (characterRunTimes.TryGetValue(data, out CharacterRunTime runTime))
            return runTime;

        return null;
    }

    public bool SetCurrentCharacter(CharacterData data)
    {
        if (!characterRunTimes.TryGetValue(data, out CharacterRunTime runTime))
            return false;

        if (runTime.IsDead)
            return false;

        currentCharacter = runTime;

        CharacterStat stat = statCalculator.ApplyStat(currentCharacter);
        currentCharacter.SetStat(stat);

        characterView.ChangeView(data);

        // 기존 모델 교체와 스탯 적용이 끝난 후 현재 캐릭터 변경 알림.
        OnCurrentCharacterChanged?.Invoke(currentCharacter);
        return true;
    }

    private void UpdateCharacterCoolDown()
    {
        foreach (CharacterRunTime runTime in characterRunTimes.Values)
            runTime.UpdateCoolDown(Time.deltaTime);
    }

    public bool IsPartyDead()
    {
        foreach (CharacterData partyMember in CharacterParty.Instance.PartyMembers)
        {
            CharacterRunTime runTime = GetCharacter(partyMember);

            if (runTime != null && !runTime.IsDead)
                return false;
        }

        return true;
    }

    public void RespawnParty()
    {
        foreach (CharacterData partyMember in CharacterParty.Instance.PartyMembers)
        {
            CharacterRunTime runTime = GetCharacter(partyMember);

            if (runTime == null)
                continue;

            runTime.Respawn();
        }

        // 첫 번째 파티원을 현재 캐릭터로
        if (CharacterParty.Instance.PartyMembers.Count > 0)
            SetCurrentCharacter(CharacterParty.Instance.PartyMembers[0]);
    }

    public IReadOnlyList<CharacterData> CharacterDatas => characterDatas;

    public CharacterData GetCharacterData(string characterId)
    {
        foreach (CharacterData data in characterDatas)
        {
            if (data == null)
                continue;

            if (data.ID == characterId)
                return data;
        }

        return null;
    }

    public void RefreshCharacterStat(CharacterRunTime runTime)
    {
        if (runTime == null)
            return;

        CharacterStat stat = statCalculator.ApplyStat(runTime);
        runTime.SetStat(stat);
    }
}
