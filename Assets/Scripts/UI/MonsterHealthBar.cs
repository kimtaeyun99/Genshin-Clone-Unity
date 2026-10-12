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

    private MonsterHealth monsterHealth;
    private Camera mainCamera;

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

        // 원소가 붙어있어도
        // 몬스터를 인식하지 않은 상태면 숨김
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