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
    [SerializeField] private Image delayHealthBarFill;

    [Header("HP Delay")]
    [SerializeField] private float delayTime = 0.3f;
    [SerializeField] private float smoothTime = 0.3f;

    [Header("Element")]
    [SerializeField] private Image hostElementIcon;
    [SerializeField] private Sprite pyroIcon;
    [SerializeField] private Sprite hydroIcon;
    [SerializeField] private Sprite cryoIcon;
    [SerializeField] private Sprite electroIcon;

    private MonsterHealth currentBoss;

    private float targetFillAmount;
    private float delayTimer;
    private float delayVelocity;

    private void Awake()
    {
        Instance = this;

        gameObject.SetActive(false);
    }

    private void Update()
    {
        UpdateDelayHealth();
    }

    public void ShowBoss(
        MonsterHealth boss,
        string bossName)
    {
        if (currentBoss == boss)
        {
            return;
        }

        if (currentBoss != null)
        {
            currentBoss.OnHPChanged -= UpdateHealth;
            currentBoss.OnHostElementChanged -= UpdateElementIcon;
            currentBoss.OnDead -= HideBoss;
        }

        currentBoss = boss;

        bossNameText.text = bossName;

        float hpPercent =
            currentBoss.MaxHP > 0f
                ? currentBoss.CurrentHP / currentBoss.MaxHP
                : 0f;

        healthBarFill.fillAmount = hpPercent;
        delayHealthBarFill.fillAmount = hpPercent;
        targetFillAmount = hpPercent;

        delayTimer = 0f;
        delayVelocity = 0f;

        UpdateElementIcon(
            currentBoss.HostElement
        );

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
        float newFillAmount =
            maxHP > 0f
                ? currentHP / maxHP
                : 0f;

        // 체력 회복
        if (newFillAmount > targetFillAmount)
        {
            healthBarFill.fillAmount =
                newFillAmount;

            delayHealthBarFill.fillAmount =
                newFillAmount;

            targetFillAmount =
                newFillAmount;

            delayTimer = 0f;
            delayVelocity = 0f;

            return;
        }

        // 데미지
        targetFillAmount =
            newFillAmount;

        // 초록색 체력은 즉시 감소
        healthBarFill.fillAmount =
            targetFillAmount;

        // 주황색 체력은 잠시 후 감소
        delayTimer = delayTime;
        delayVelocity = 0f;
    }

    private void UpdateDelayHealth()
    {
        if (delayHealthBarFill.fillAmount <= targetFillAmount)
        {
            delayHealthBarFill.fillAmount =
                targetFillAmount;

            return;
        }

        if (delayTimer > 0f)
        {
            delayTimer -= Time.deltaTime;
            return;
        }

        delayHealthBarFill.fillAmount =
            Mathf.SmoothDamp(
                delayHealthBarFill.fillAmount,
                targetFillAmount,
                ref delayVelocity,
                smoothTime
            );

        if (Mathf.Abs(
                delayHealthBarFill.fillAmount -
                targetFillAmount) < 0.001f)
        {
            delayHealthBarFill.fillAmount =
                targetFillAmount;

            delayVelocity = 0f;
        }
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