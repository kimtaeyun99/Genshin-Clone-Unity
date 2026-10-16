using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartySlotUI : MonoBehaviour
{
    [Header("Character")]
    [SerializeField] private Image characterIcon;
    [SerializeField] private TMP_Text characterNameText;

    [Header("Health")]
    [SerializeField] private Image healthBarFill;

    [Header("Selected")]
    [SerializeField] private GameObject selectedEffect;

    [Header("Switch CoolDown")]
    [SerializeField] private Image switchCoolDownFill;
    [SerializeField] private TMP_Text switchCoolDownText;

    private CharacterData displayedCharacter;
    private int lastCurrentHP = int.MinValue;
    private int lastMaxHP = int.MinValue;
    private int lastCooldownTenths = int.MinValue;

    public void SetCharacter(CharacterData data, CharacterRunTime runTime)
    {
        if (data == null)
        {
            displayedCharacter = null;
            lastCurrentHP = int.MinValue;
            lastMaxHP = int.MinValue;

            if (gameObject.activeSelf)
                gameObject.SetActive(false);
            return;
        }

        if (!gameObject.activeSelf)
            gameObject.SetActive(true);

        if (displayedCharacter != data)
        {
            displayedCharacter = data;
            characterIcon.sprite = data.CharacterIcon;
            characterNameText.text = data.CharacterName;
            lastCurrentHP = int.MinValue;
            lastMaxHP = int.MinValue;
        }

        if (runTime != null)
            SetHealth(runTime.CurrentHP, runTime.MaxHP);
        else
            SetHealth(0, 0);
    }

    public void SetHealth(int currentHP, int maxHP)
    {
        if (lastCurrentHP == currentHP && lastMaxHP == maxHP)
            return;

        healthBarFill.fillAmount = maxHP > 0
            ? Mathf.Clamp01((float)currentHP / maxHP)
            : 0f;

        characterIcon.color = currentHP <= 0
            ? Color.gray
            : Color.white;

        lastCurrentHP = currentHP;
        lastMaxHP = maxHP;
    }

    public void SetSelected(bool isSelected)
    {
        if (selectedEffect.activeSelf != isSelected)
            selectedEffect.SetActive(isSelected);
    }

    public void SetSwitchCoolDown(float currentCoolDown, float maxCoolDown)
    {
        bool isCoolingDown = currentCoolDown > 0f && maxCoolDown > 0f;

        if (switchCoolDownFill.gameObject.activeSelf != isCoolingDown)
            switchCoolDownFill.gameObject.SetActive(isCoolingDown);

        if (switchCoolDownText.gameObject.activeSelf != isCoolingDown)
            switchCoolDownText.gameObject.SetActive(isCoolingDown);

        if (!isCoolingDown)
        {
            lastCooldownTenths = int.MinValue;
            return;
        }

        switchCoolDownFill.fillAmount = Mathf.Clamp01(currentCoolDown / maxCoolDown);

        int currentTenths = Mathf.RoundToInt(currentCoolDown * 10f);
        if (lastCooldownTenths != currentTenths)
        {
            switchCoolDownText.text = currentCoolDown.ToString("F1");
            lastCooldownTenths = currentTenths;
        }
    }
}
