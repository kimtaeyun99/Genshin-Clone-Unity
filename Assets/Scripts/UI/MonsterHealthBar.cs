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

    private MonsterController monster;
    private MonsterRunTime runtime;
    private Health health;
    private Camera mainCamera;

    private float targetFillAmount;
    private float delayTimer;
    private float delayVelocity;

    // HP 변화가 발생한 동안에만 지연 체력바를 보간한다.
    private bool isDelayAnimating;
    private bool isVisible;

    private void Awake()
    {
        monster = GetComponentInParent<MonsterController>();
        mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        Subscribe();
    }

    private void Start()
    {
        // 다른 MonoBehaviour의 Awake 순서가 보장되지 않으므로 Start에서도 연결을 확인한다.
        Subscribe();
        if (runtime == null || health == null)
            return;

        float hpPercent = runtime.MaxHP > 0f
            ? runtime.CurrentHP / runtime.MaxHP
            : 0f;

        hpFill.fillAmount = hpPercent;
        delayHpFill.fillAmount = hpPercent;
        targetFillAmount = hpPercent;
        isDelayAnimating = false;

        UpdateElementIcon(health.HostElement);
        SetVisible(false);
    }

    private void Subscribe()
    {
        if (runtime != null || monster == null || monster.Runtime == null ||
            monster.Health == null)
            return;

        runtime = monster.Runtime;
        health = monster.Health;
        runtime.OnHPChanged += UpdateHP;
        health.OnHostElementChanged += UpdateElementIcon;
    }

    private void Update()
    {
        if (isDelayAnimating)
            UpdateDelayHP();
    }

    private void LateUpdate()
    {
        // 숨겨진 HP 바는 카메라 방향으로 회전할 필요가 없다.
        if (!isVisible)
            return;

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null)
                return;
        }

        transform.rotation = Quaternion.LookRotation(
            transform.position - mainCamera.transform.position);
    }

    public void SetVisible(bool visible)
    {
        Subscribe();
        isVisible = visible;
        healthBarBG.SetActive(visible);
        hpFill.gameObject.SetActive(visible);
        delayHpFill.gameObject.SetActive(visible);

        if (!visible)
        {
            hostElementIcon.gameObject.SetActive(false);
        }
        else if (health != null)
        {
            UpdateElementIcon(health.HostElement);
        }
    }

    private void UpdateHP(float currentHP, float maxHP)
    {
        float newFillAmount = maxHP > 0f ? currentHP / maxHP : 0f;

        if (newFillAmount > targetFillAmount)
        {
            hpFill.fillAmount = newFillAmount;
            delayHpFill.fillAmount = newFillAmount;
            targetFillAmount = newFillAmount;
            delayVelocity = 0f;
            delayTimer = 0f;
            isDelayAnimating = false;
            return;
        }

        targetFillAmount = newFillAmount;
        hpFill.fillAmount = targetFillAmount;
        delayTimer = delayTime;
        delayVelocity = 0f;
        isDelayAnimating = delayHpFill.fillAmount > targetFillAmount;
    }

    private void UpdateDelayHP()
    {
        if (delayHpFill.fillAmount <= targetFillAmount)
        {
            delayHpFill.fillAmount = targetFillAmount;
            isDelayAnimating = false;
            return;
        }

        if (delayTimer > 0f)
        {
            delayTimer -= Time.deltaTime;
            return;
        }

        delayHpFill.fillAmount = Mathf.SmoothDamp(
            delayHpFill.fillAmount,
            targetFillAmount,
            ref delayVelocity,
            smoothTime
        );

        if (Mathf.Abs(delayHpFill.fillAmount - targetFillAmount) < 0.001f)
        {
            delayHpFill.fillAmount = targetFillAmount;
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
                hostElementIcon.gameObject.SetActive(false);
                return;
        }

        hostElementIcon.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        if (runtime != null)
            runtime.OnHPChanged -= UpdateHP;
        if (health != null)
            health.OnHostElementChanged -= UpdateElementIcon;

        runtime = null;
        health = null;
    }
}
