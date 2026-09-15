using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "GameData/Character")]
public class CharacterData : ScriptableObject
{
    [Header("Identity")]
    [SerializeField] private string id;
    [SerializeField] private string characterName;
    [SerializeField] private GameObject prefab;

    [Header("Attack")]
    [SerializeField] private ElementType elementType;
    [SerializeField] private WeaponType weaponType;

    [Header("Basic Stat")]
    [SerializeField] private int hp;
    [SerializeField] private int hpPerLevel;

    [SerializeField] private int atk;
    [SerializeField] private int atkPerLevel;

    [SerializeField] private int def;
    [SerializeField] private int defPerLevel;

    [SerializeField] private int elementalMastery;

    [Header("Ascension Stat")]
    [SerializeField] private StatType ascensionStatType;
    [SerializeField] private CharacterAscensionData ascensionData;

    public string ID => id;
    public string CharacterName => characterName;
    public GameObject Prefab => prefab;
    public ElementType ElementType => elementType;
    public WeaponType WeaponType => weaponType;
    public int HP => hp;
    public int HPPerLevel => hpPerLevel;
    public int ATK => atk;
    public int ATKPerLevel => atkPerLevel;
    public int DEF => def;
    public int DEFPerLevel => defPerLevel;
    public int ElementalMastery => elementalMastery;
    public StatType AscensionStatType => ascensionStatType;
    public CharacterAscensionData AscensionData => ascensionData;
    public void SetData(
    string id,
    string characterName,
    GameObject prefab,
    ElementType elementType,
    WeaponType weaponType,
    int hp,
    int hpPerLevel,
    int atk,
    int atkPerLevel,
    int def,
    int defPerLevel,
    int elementalMastery,
    StatType ascensionStatType,
    CharacterAscensionData ascensionData)
    {
        this.id = id;
        this.characterName = characterName;
        this.prefab = prefab;

        this.elementType = elementType;
        this.weaponType = weaponType;

        this.hp = hp;
        this.hpPerLevel = hpPerLevel;

        this.atk = atk;
        this.atkPerLevel = atkPerLevel;

        this.def = def;
        this.defPerLevel = defPerLevel;

        this.elementalMastery = elementalMastery;

        this.ascensionStatType = ascensionStatType;

        this.ascensionData = ascensionData;
    }
}
