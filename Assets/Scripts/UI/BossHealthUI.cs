using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthUI : MonoBehaviour
{
    public static BossHealthUI Instance
    {
        get;
        private set;
    }

    [Header("HP")]
    [SerializeField] private TMP_Text bossNameText;
    [SerializeField] private Image healthBarFill;

    [Header("Element")]
    [SerializeField] private Image hostElementIcon;
    [SerializeField] private Sprite pyroIcon;
    [SerializeField] private Sprite hydroIcon;
    [SerializeField] private Sprite cryoIcon;
    [SerializeField] private Sprite electroIcon;

    private MonsterHealth currentBoss;

    private void Awake()
    {
        Instance = this;

        gameObject.SetActive(false);
    }

    public void ShowBoss(
        MonsterHealth boss,
        string bossName)
    {
        if (currentBoss == boss)
        {
            return;
        }

        // 기존 보스가 있으면 이벤트 해제
        if (currentBoss != null)
        {
            currentBoss.OnHPChanged -= UpdateHealth;
            currentBoss.OnHostElementChanged -= UpdateElementIcon;
            currentBoss.OnDead -= HideBoss;
        }

        currentBoss = boss;

        bossNameText.text = bossName;

        // 현재 HP 즉시 반영
        UpdateHealth(
            currentBoss.CurrentHP,
            currentBoss.MaxHP
        );

        // 현재 부착 원소 즉시 반영
        UpdateElementIcon(
            currentBoss.HostElement
        );

        // 이벤트 구독
        currentBoss.OnHPChanged += UpdateHealth;
        currentBoss.OnHostElementChanged += UpdateElementIcon;
        currentBoss.OnDead += HideBoss;

        gameObject.SetActive(true);
    }

    public void HideBoss()
    {
        if (currentBoss != null)
        {
            currentBoss.OnHPChanged -= UpdateHealth;
            currentBoss.OnHostElementChanged -= UpdateElementIcon;
            currentBoss.OnDead -= HideBoss;

            currentBoss = null;
        }

        hostElementIcon.gameObject.SetActive(false);

        gameObject.SetActive(false);
    }

    private void UpdateHealth(
        float currentHP,
        float maxHP)
    {
        healthBarFill.fillAmount =
            maxHP > 0f
                ? currentHP / maxHP
                : 0f;
    }

    private void UpdateElementIcon(
        ElementType elementType)
    {
        switch (elementType)
        {
            case ElementType.Pyro:
                hostElementIcon.sprite = pyroIcon;
                break;

            case ElementType.Hydro:
                hostElementIcon.sprite = hydroIcon;
                break;

            case ElementType.Cryo:
                hostElementIcon.sprite = cryoIcon;
                break;

            case ElementType.Electro:
                hostElementIcon.sprite = electroIcon;
                break;

            default:
                hostElementIcon.sprite = null;
                hostElementIcon.gameObject.SetActive(false);
                return;
        }

        hostElementIcon.gameObject.SetActive(true);
    }

    private void OnDestroy()
    {
        if (currentBoss != null)
        {
            currentBoss.OnHPChanged -= UpdateHealth;
            currentBoss.OnHostElementChanged -= UpdateElementIcon;
            currentBoss.OnDead -= HideBoss;
        }

        if (Instance == this)
        {
            Instance = null;
        }
    }
}