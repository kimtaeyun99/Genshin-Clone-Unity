
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CooldownUI : MonoBehaviour
{
    [Header("Icon")]
    [SerializeField] private Image background;
    [SerializeField] private Image fill;

    [Header("Cooldown")]
    [SerializeField] private TMP_Text cooldownText;

    private int lastTenths = int.MinValue;

    public void SetIcon(Sprite icon)
    {
        background.sprite = icon;
        fill.sprite = icon;
    }

    public void SetCooldown(float remaining, float total)
    {
        fill.fillAmount = total > 0f
            ? Mathf.Clamp01(1f - remaining / total)
            : 1f;

        bool isCoolingDown = remaining > 0f;

        if (cooldownText.gameObject.activeSelf != isCoolingDown)
            cooldownText.gameObject.SetActive(isCoolingDown);

        if (!isCoolingDown)
        {
            lastTenths = int.MinValue;
            return;
        }

        int tenths = Mathf.RoundToInt(remaining * 10f);

        if (lastTenths == tenths)
            return;

        cooldownText.text = (tenths / 10f).ToString("0.0");
        lastTenths = tenths;
    }

    public void ResetCache()
    {
        lastTenths = int.MinValue;
    }
}
