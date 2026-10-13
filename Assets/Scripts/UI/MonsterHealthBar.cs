using UnityEngine;
using UnityEngine.UI;

public class MonsterHealthBar : MonoBehaviour
{
    [Header("HP")]
    [SerializeField] private GameObject healthBarBG;
    [SerializeField] private Image hpFill;
    [SerializeField] private Image delayHpFill;

    [Header("HP Delay")]
    [SerializeField] private float delayTime = 0.3f;
    [SerializeField] private float smoothTime = 0.3f;

    [Header("Element")]
    [SerializeField] private Image hostElementIcon;
    [SerializeField] private Sprite pyroIcon;
    [SerializeField] private Sprite hydroIcon;
    [SerializeField] private Sprite cryoIcon;
    [SerializeField] private Sprite electroIcon;

    private MonsterHealth monsterHealth;
    private Camera mainCamera;

    private float targetFillAmount;
    private float delayTimer;
    private float delayVelocity;

    private void Awake()
    {
        monsterHealth =
            GetComponentInParent<MonsterHealth>();

        mainCamera = Camera.main;
    }

    private void Start()
    {
        float hpPercent =
            monsterHealth.MaxHP > 0f
                ? monsterHealth.CurrentHP / monsterHealth.MaxHP
                : 0f;

        hpFill.fillAmount = hpPercent;
        delayHpFill.fillAmount = hpPercent;

        targetFillAmount = hpPercent;

        UpdateElementIcon(
            monsterHealth.HostElement
        );

        SetVisible(false);
    }

    private void OnEnable()
    {
        if (monsterHealth == null)
        {
            return;
        }

        monsterHealth.OnHPChanged += UpdateHP;
        monsterHealth.OnHostElementChanged += UpdateElementIcon;
    }

    private void Update()
    {
        UpdateDelayHP();
    }

    private void LateUpdate()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;

            if (mainCamera == null)
            {
                return;
            }
        }

        transform.rotation =
            Quaternion.LookRotation(
                transform.position -
                mainCamera.transform.position
            );
    }

    public void SetVisible(bool visible)
    {
        healthBarBG.SetActive(visible);
        hpFill.gameObject.SetActive(visible);
        delayHpFill.gameObject.SetActive(visible);

        if (!visible)
        {
            hostElementIcon.gameObject.SetActive(false);
        }
        else
        {
            UpdateElementIcon(
                monsterHealth.HostElement
            );
        }
    }

    private void UpdateHP(
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
            hpFill.fillAmount = newFillAmount;
            delayHpFill.fillAmount = newFillAmount;

            targetFillAmount = newFillAmount;

            delayVelocity = 0f;
            delayTimer = 0f;

            return;
        }

        // 데미지
        targetFillAmount = newFillAmount;

        // 초록색은 즉시 감소
        hpFill.fillAmount = targetFillAmount;

        // 주황색은 잠시 후 감소
        delayTimer = delayTime;
        delayVelocity = 0f;
    }

    private void UpdateDelayHP()
    {
        // 이미 현재 HP까지 도착
        if (delayHpFill.fillAmount <= targetFillAmount)
        {
            delayHpFill.fillAmount = targetFillAmount;
            return;
        }

        // 피격 후 잠깐 기다림
        if (delayTimer > 0f)
        {
            delayTimer -= Time.deltaTime;
            return;
        }

        // 현재 HP까지 부드럽게 감소
        delayHpFill.fillAmount =
            Mathf.SmoothDamp(
                delayHpFill.fillAmount,
                targetFillAmount,
                ref delayVelocity,
                smoothTime
            );

        if (Mathf.Abs(
                delayHpFill.fillAmount -
                targetFillAmount) < 0.001f)
        {
            delayHpFill.fillAmount =
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
                hostElementIcon.gameObject.SetActive(false);
                return;
        }

        hostElementIcon.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        if (monsterHealth == null)
        {
            return;
        }

        monsterHealth.OnHPChanged -= UpdateHP;
        monsterHealth.OnHostElementChanged -= UpdateElementIcon;
    }
}