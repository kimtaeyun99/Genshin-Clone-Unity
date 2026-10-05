using UnityEngine;
using System.Collections.Generic;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager Instance { get; private set; }


    [Header("Character Data")]
    [SerializeField] private List<CharacterData> characterDatas;

    [Header("View")]
    [SerializeField] private CharacterView characterView;

    [Header("Animator")]
    public Animator CurrentAnimator => characterView.CurrentAnimator;
    [Header("AttackHitbox")]
    public AttackHitbox CurrentAttackHitbox => characterView.CurrentAttackHitbox;

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
        if(Instance != null && Instance != this)
        {
            Destroy(Instance);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        statCalculator = new CharacterStatCalculator();
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

        CharacterStat stat = statCalculator.ApplyStat(currentCharacter);
        currentCharacter.SetStat(stat);

        characterView.ChangeView(data);

        return true;
    }
}
