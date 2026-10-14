using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StaminaUI : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private PlayerStamina playerStamina;

    [Header("UI")]
    [SerializeField] private Image staminaBarFill;
    [SerializeField] private TMP_Text currentStaminaText;
    [SerializeField] private TMP_Text maxStaminaText;

    [Header("Color")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color lowStaminaColor = Color.red;
    [SerializeField] private float lowStaminaPercent = 0.2f;

    private int lastDisplayedCurrent = int.MinValue;
    private int lastDisplayedMax = int.MinValue;

    private void OnEnable()
    {
        if (playerStamina != null)
        {
            playerStamina.OnStaminaChanged += UpdateStaminaUI;
            UpdateStaminaUI(playerStamina.CurrentStamina, playerStamina.MaxStamina);
        }
    }

    private void Start()
    {
        if (playerStamina != null)
        {
            UpdateStaminaUI(playerStamina.CurrentStamina, playerStamina.MaxStamina);
        }
    }

    private void OnDisable()
    {
        if (playerStamina != null)
        {
            playerStamina.OnStaminaChanged -= UpdateStaminaUI;
        }
    }

    private void UpdateStaminaUI(float currentStamina, float maxStamina)
    {
        if (staminaBarFill == null || currentStaminaText == null || maxStaminaText == null)
        {
            return;
        }

        float staminaPercent = maxStamina > 0f
            ? Mathf.Clamp01(currentStamina / maxStamina)
            : 0f;

        staminaBarFill.fillAmount = staminaPercent;
        staminaBarFill.color = staminaPercent <= lowStaminaPercent
            ? lowStaminaColor
            : normalColor;

        int displayedCurrent = Mathf.CeilToInt(currentStamina);
        if (lastDisplayedCurrent != displayedCurrent)
        {
            currentStaminaText.SetText("{0}", displayedCurrent);
            lastDisplayedCurrent = displayedCurrent;
        }

        int displayedMax = Mathf.CeilToInt(maxStamina);
        if (lastDisplayedMax != displayedMax)
        {
            maxStaminaText.SetText("{0}", displayedMax);
            lastDisplayedMax = displayedMax;
        }
    }
}
