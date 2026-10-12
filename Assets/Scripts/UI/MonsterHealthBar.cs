using UnityEngine;
using UnityEngine.UI;

public class MonsterHealthBar : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject healthBarBG;
    [SerializeField] private Image hpFill;

    [Header("Display")]
    [SerializeField] private float visibleDuration = 3f;

    private MonsterHealth monsterHealth;
    private Camera mainCamera;

    private float visibleTimer;
    private bool isVisible;

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

        SetVisible(false);
    }

    private void OnEnable()
    {
        if (monsterHealth != null)
        {
            monsterHealth.OnHPChanged += HandleHPChanged;
        }
    }

    private void Update()
    {
        if (!isVisible)
        {
            return;
        }

        visibleTimer -= Time.deltaTime;

        if (visibleTimer <= 0f)
        {
            SetVisible(false);
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

        SetVisible(true);
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

    private void SetVisible(bool visible)
    {
        isVisible = visible;

        healthBarBG.SetActive(visible);
        hpFill.gameObject.SetActive(visible);
    }

    private void OnDisable()
    {
        if (monsterHealth != null)
        {
            monsterHealth.OnHPChanged -= HandleHPChanged;
        }
    }
}