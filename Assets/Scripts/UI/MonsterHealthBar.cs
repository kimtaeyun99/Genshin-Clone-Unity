using UnityEngine;
using UnityEngine.UI;

public class MonsterHealthBar : MonoBehaviour
{
    [Header("HP")]
    [SerializeField] private GameObject healthBarBG;
    [SerializeField] private Image hpFill;

    [Header("Element")]
    [SerializeField] private Image hostElementIcon;
    [SerializeField] private Sprite pyroIcon;
    [SerializeField] private Sprite hydroIcon;
    [SerializeField] private Sprite cryoIcon;
    [SerializeField] private Sprite electroIcon;

    [Header("Display")]
    [SerializeField] private float visibleDuration = 3f;

    private MonsterHealth monsterHealth;
    private Camera mainCamera;

    private float visibleTimer;
    private bool isHealthBarVisible;

    private void Awake()
    {
        monsterHealth =
            GetComponentInParent<MonsterHealth>();

        mainCamera = Camera.main;
    }

    private void Start()
    {
        UpdateHP(
            monsterHealth.CurrentHP,
            monsterHealth.MaxHP
        );

        UpdateElementIcon(
            monsterHealth.HostElement
        );

        SetHealthBarVisible(false);
    }

    private void OnEnable()
    {
        if (monsterHealth == null)
        {
            return;
        }

        monsterHealth.OnHPChanged += HandleHPChanged;
        monsterHealth.OnHostElementChanged += UpdateElementIcon;
    }

    private void Update()
    {
        if (!isHealthBarVisible)
        {
            return;
        }

        visibleTimer -= Time.deltaTime;

        if (visibleTimer <= 0f)
        {
            SetHealthBarVisible(false);
        }
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

    private void HandleHPChanged(
        float currentHP,
        float maxHP)
    {
        UpdateHP(currentHP, maxHP);

        visibleTimer = visibleDuration;

        SetHealthBarVisible(true);
    }

    private void UpdateHP(
        float currentHP,
        float maxHP)
    {
        hpFill.fillAmount =
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
                hostElementIcon.gameObject.SetActive(false);
                return;
        }

        hostElementIcon.gameObject.SetActive(true);
    }

    private void SetHealthBarVisible(bool visible)
    {
        isHealthBarVisible = visible;

        healthBarBG.SetActive(visible);
        hpFill.gameObject.SetActive(visible);
    }

    private void OnDisable()
    {
        if (monsterHealth == null)
        {
            return;
        }

        monsterHealth.OnHPChanged -= HandleHPChanged;
        monsterHealth.OnHostElementChanged -= UpdateElementIcon;
    }
}