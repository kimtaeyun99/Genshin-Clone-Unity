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

    [Header("UI")]
    [SerializeField] private TMP_Text bossNameText;
    [SerializeField] private Image healthBarFill;

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

        if (currentBoss != null)
        {
            currentBoss.OnHPChanged -= UpdateHealth;
            currentBoss.OnDead -= HideBoss;
        }

        currentBoss = boss;

        bossNameText.text = bossName;

        UpdateHealth(
            currentBoss.CurrentHP,
            currentBoss.MaxHP
        );

        currentBoss.OnHPChanged += UpdateHealth;
        currentBoss.OnDead += HideBoss;

        gameObject.SetActive(true);
    }

    public void HideBoss()
    {
        if (currentBoss != null)
        {
            currentBoss.OnHPChanged -= UpdateHealth;
            currentBoss.OnDead -= HideBoss;

            currentBoss = null;
        }

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

    private void OnDestroy()
    {
        if (currentBoss != null)
        {
            currentBoss.OnHPChanged -= UpdateHealth;
            currentBoss.OnDead -= HideBoss;
        }

        if (Instance == this)
        {
            Instance = null;
        }
    }
}