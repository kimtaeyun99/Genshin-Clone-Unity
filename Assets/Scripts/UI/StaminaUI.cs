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

    private void Start()
    {
        UpdateStaminaUI();
    }

    private void Update()
    {
        UpdateStaminaUI();
    }

    private void UpdateStaminaUI()
    {
        if (playerStamina == null)
        {
            return;
        }

        float currentStamina = playerStamina.CurrentStamina;
        float maxStamina = playerStamina.MaxStamina;

        float staminaPercent =
            currentStamina / maxStamina;

        staminaBarFill.fillAmount =
            staminaPercent;

        currentStaminaText.text =
            Mathf.CeilToInt(currentStamina).ToString();

        maxStaminaText.text =
            Mathf.CeilToInt(maxStamina).ToString();

        if (staminaPercent <= lowStaminaPercent)
        {
            staminaBarFill.color =
                lowStaminaColor;
        }
        else
        {
            staminaBarFill.color =
                normalColor;
        }
    }
}