using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthUI : MonoBehaviour
{
    public static BossHealthUI Instance { get; private set; }

    [Header("HP")]
    [SerializeField] private TMP_Text bossNameText;

    [Header("Element")]
    [SerializeField] private Image hostElementIcon;
    [SerializeField] private Sprite pyroIcon;
    [SerializeField] private Sprite hydroIcon;
    [SerializeField] private Sprite cryoIcon;
    [SerializeField] private Sprite electroIcon;

    [Header("HP")]
    [SerializeField] private DelayedHealthBar delayedHealthBar;

    private MonsterRunTime currentBoss;
    private Health currentHealth;

    private void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

    public void ShowBoss(MonsterRunTime boss, Health health, string bossName)
    {
        if (currentBoss == boss)
            return;

        Unsubscribe();
        currentBoss = boss;
        currentHealth = health;

        bossNameText.text = bossName;
        delayedHealthBar.Initialize(currentBoss.CurrentHP,currentBoss.MaxHP);

        UpdateElementIcon(currentHealth.HostElement);
        currentBoss.OnHPChanged += UpdateHealth;
        currentBoss.OnDead += HideBoss;
        currentHealth.OnHostElementChanged += UpdateElementIcon;
        gameObject.SetActive(true);
    }

    public void HideBoss(MonsterRunTime boss)
    {
        if (currentBoss != boss)
            return;

        Unsubscribe();

        hostElementIcon.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    private void Unsubscribe()
    {
        if (currentBoss != null)
        {
            currentBoss.OnHPChanged -= UpdateHealth;
            currentBoss.OnDead -= HideBoss;
            currentBoss = null;
        }

        if (currentHealth != null)
        {
            currentHealth.OnHostElementChanged -= UpdateElementIcon;
            currentHealth = null;
        }
    }

    private void UpdateHealth(float currentHP, float maxHP)
    {
        delayedHealthBar.SetHealth(currentHP, maxHP);
    }

    private void UpdateElementIcon(ElementType elementType)
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
        Unsubscribe();
        if (Instance == this)
            Instance = null;
    }
}
