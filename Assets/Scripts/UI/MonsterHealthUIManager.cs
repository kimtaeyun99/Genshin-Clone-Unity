//using UnityEngine;

//public class MonsterHealthUIManager : MonoBehaviour
//{
//    public static MonsterHealthUIManager Instance
//    {
//        get;
//        private set;
//    }

//    [SerializeField]
//    private MonsterHealthBar healthBarPrefab;

//    [SerializeField]
//    private RectTransform healthBarRoot;

//    private void Awake()
//    {
//        Instance = this;
//    }

//    public void CreateHealthBar(
//        MonsterHealth monsterHealth)
//    {
//        MonsterHealthBar healthBar =
//            Instantiate(
//                healthBarPrefab,
//                healthBarRoot
//            );

//        healthBar.Initialize(
//            monsterHealth
//        );
//    }
//}