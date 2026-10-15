
using UnityEngine;
using UnityEngine.UI;

public class DelayedHealthBar : MonoBehaviour
{
    [Header("HP")]
    [SerializeField] private Image healthBarFill;
    [SerializeField] private Image delayHealthBarFill;

    [Header("Delay")]
    [SerializeField] private float delayTime = 0.3f;
    [SerializeField] private float smoothTime = 0.3f;

    private float targetFillAmount;
    private float delayTimer;
    private float delayVelocity;
    private bool isAnimating;

    private void Update()
    {
        if (!isAnimating)
            return;

        UpdateDelayHealth();
    }

    public void Initialize(float currentHP, float maxHP)
    {
        float fill = GetFillAmount(currentHP, maxHP);

        healthBarFill.fillAmount = fill;
        delayHealthBarFill.fillAmount = fill;

        targetFillAmount = fill;
        delayTimer = 0f;
        delayVelocity = 0f;
        isAnimating = false;
    }

    public void SetHealth(float currentHP, float maxHP)
    {
        float fill = GetFillAmount(currentHP, maxHP);

        if (fill >= targetFillAmount)
        {
            Initialize(currentHP, maxHP);
            return;
        }

        targetFillAmount = fill;
        healthBarFill.fillAmount = fill;

        delayTimer = delayTime;
        delayVelocity = 0f;

        isAnimating =
            delayHealthBarFill.fillAmount > targetFillAmount;
    }

    private void UpdateDelayHealth()
    {
        if (delayHealthBarFill.fillAmount <= targetFillAmount)
        {
            delayHealthBarFill.fillAmount = targetFillAmount;
            isAnimating = false;
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

        if (Mathf.Abs(
            delayHealthBarFill.fillAmount - targetFillAmount) < 0.001f)
        {
            delayHealthBarFill.fillAmount = targetFillAmount;
            delayVelocity = 0f;
            isAnimating = false;
        }
    }

    private static float GetFillAmount(float currentHP, float maxHP)
    {
        return maxHP > 0f
            ? Mathf.Clamp01(currentHP / maxHP)
            : 0f;
    }
}
