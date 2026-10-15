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
    [SerializeField] private float[] attackMultipliers;

    [Header("Skill")]
    [SerializeField] private float[] skillMultipliers;
    [SerializeField] private float skillCoolDown;

    [Header("Burst")]
    [SerializeField] private int burstEnergyCost;
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
    public float[] AttackMultipliers => attackMultipliers;
    public float[] SkillMultipliers => skillMultipliers;
    public float SkillCoolDown => skillCoolDown;
    public int BurstEnergyCost => burstEnergyCost;
    public float[] BurstMultipliers => burstMultipliers;
    public float BurstCoolDown => burstCoolDown;
    public Avatar Avatar => avatar;
    public RuntimeAnimatorController AnimatorController => animatorController;
}
