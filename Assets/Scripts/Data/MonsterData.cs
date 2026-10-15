using UnityEngine;

[CreateAssetMenu(fileName = "MonsterData", menuName = "GameData/Monster")]
public class MonsterData : ScriptableObject
{
    [Header("Identity")]
    [SerializeField] private string monsterName;
    [SerializeField] private ElementType elementType;

    [Header("Basic Stat")]
    [SerializeField] private int atk;
    [SerializeField] private int maxHp;

    [Header("Attack")]
    [SerializeField] private IMonsterAttack monsterAttack;

    [Header("Animation")]
    [SerializeField] private Avatar avatar;
    [SerializeField] private RuntimeAnimatorController animatorController;

    public string MonsterName => monsterName;
    public ElementType ElementType => elementType;
    public int ATK => atk;
    public int MaxHP => maxHp;
    public IMonsterAttack MonsterAttack => monsterAttack;
    public Avatar Avatar => avatar;
    public RuntimeAnimatorController AnimatorController => animatorController;
}
