using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "GameData/Character")]
public class CharacterData : ScriptableObject
{
    [Header("Identity")]
    [SerializeField] private string id;
    [SerializeField] private string characterName;
    [SerializeField] private GameObject prefab;
    [SerializeField] private ElementType elementType;
    [SerializeField] private Sprite characterIcon;
    [SerializeField] private Sprite skillIcon;
    [SerializeField] private Sprite burstIcon;

    [Header("Basic Stat")]
    [SerializeField] private int atk;
    [SerializeField] private int maxHp;

    [Header("Attack")]
    [SerializeField] private ICharacterAttack characterAttack;
    [SerializeField] private float[] attackMultipliers;

    [Header("Skill")]
    [SerializeField] private ICharacterSkill characterSkill;
    [SerializeField] private float[] skillMultipliers;
    [SerializeField] private float skillCoolDown;

    [Header("Burst")]
    [SerializeField] private int burstEnergyCost;
    [SerializeField] private ICharacterBurst characterBurst;
    [SerializeField] private float[] burstMultipliers;
    [SerializeField] private float burstCoolDown;

    [Header("Animation")]
    [SerializeField] private Avatar avatar;
    [SerializeField] private RuntimeAnimatorController animatorController;

    public string ID => id;
    public string CharacterName => characterName;
    public GameObject Prefab => prefab;
    public ElementType ElementType => elementType;
    public Sprite CharacterIcon => characterIcon;
    public Sprite SkillIcon => skillIcon;
    public Sprite BurstIcon => burstIcon;
    public int ATK => atk;
    public int MaxHP => maxHp;
    public ICharacterAttack CharacterAttack => characterAttack;
    public float[] AttackMultipliers => attackMultipliers;
    public ICharacterSkill CharacterSkill => characterSkill;
    public float[] SkillMultipliers => skillMultipliers;
    public float SkillCoolDown => skillCoolDown;
    public int BurstEnergyCost => burstEnergyCost;
    public ICharacterBurst CharacterBurst => characterBurst;
    public float[] BurstMultipliers => burstMultipliers;
    public float BurstCoolDown => burstCoolDown;
}
