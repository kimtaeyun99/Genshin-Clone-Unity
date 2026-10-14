using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthUI : MonoBehaviour
{
    public static BossHealthUI Instance { get; private set; }

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

    private MonsterRunTime currentBoss;
    private Health currentHealth;

    private float targetFillAmount;
    private float delayTimer;
    private float delayVelocity;

    private bool isDelayAnimating;

    private void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (isDelayAnimating)
            UpdateDelayHealth();
    }

    public void ShowBoss(MonsterRunTime boss, Health health, string bossName)
    {
        if (currentBoss == boss)
            return;

        Unsubscribe();
        currentBoss = boss;
        currentHealth = health;

        bossNameText.text = bossName;
        float hpPercent = currentBoss.MaxHP > 0f
            ? currentBoss.CurrentHP / currentBoss.MaxHP
            : 0f;

        healthBarFill.fillAmount = hpPercent;
        delayHealthBarFill.fillAmount = hpPercent;
        targetFillAmount = hpPercent;
        delayTimer = 0f;
        delayVelocity = 0f;
        isDelayAnimating = false;

        UpdateElementIcon(currentHealth.HostElement);
        currentBoss.OnHPChanged += UpdateHealth;
        currentBoss.OnDead += HideBoss;
        currentHealth.OnHostElementChanged += UpdateElementIcon;
        gameObject.SetActive(true);
    }

    public void HideBoss()
    {
        Unsubscribe();
        isDelayAnimating = false;
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
        float newFillAmount = maxHP > 0f ? currentHP / maxHP : 0f;

        if (newFillAmount > targetFillAmount)
        {
            healthBarFill.fillAmount = newFillAmount;
            delayHealthBarFill.fillAmount = newFillAmount;
            targetFillAmount = newFillAmount;
            delayTimer = 0f;
            delayVelocity = 0f;
            isDelayAnimating = false;
            return;
        }

        targetFillAmount = newFillAmount;
        healthBarFill.fillAmount = targetFillAmount;
        delayTimer = delayTime;
        delayVelocity = 0f;
        isDelayAnimating = delayHealthBarFill.fillAmount > targetFillAmount;
    }

    private void UpdateDelayHealth()
    {
        if (delayHealthBarFill.fillAmount <= targetFillAmount)
        {
            delayHealthBarFill.fillAmount = targetFillAmount;
            isDelayAnimating = false;
            return;
        }

        if (delayTimer > 0f)
        {
            delayTimer -= Time.deltaTime;
            return;
        }

        delayHealthBarFill.fillAmount = Mathf.SmoothDamp(
            delayHealthBarFill.fillAmount,
            targetFillAmount,
            ref delayVelocity,
            smoothTime
        );

        if (Mathf.Abs(delayHealthBarFill.fillAmount - targetFillAmount) < 0.001f)
        {
            delayHealthBarFill.fillAmount = targetFillAmount;
            delayVelocity = 0f;
            isDelayAnimating = false;
        }
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
