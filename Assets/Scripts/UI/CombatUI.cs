using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombatUI : MonoBehaviour
{
    [Header("Energy")]
    [SerializeField] private Image energyBarFill;
    [SerializeField] private TMP_Text currentEnergyText;
    [SerializeField] private TMP_Text maxEnergyText;

    [Header("Skill")]
    [SerializeField] private Image skillCooldownBG;
    [SerializeField] private Image skillCoolDownFill;
    [SerializeField] private TMP_Text skillCoolDownText;

    [Header("Burst")]
    [SerializeField] private Image burstCooldownBG;
    [SerializeField] private Image burstCoolDownFill;
    [SerializeField] private TMP_Text burstCoolDownText;

    private CharacterRunTime currentCharacter;

    private void Update()
    {
        if (CharacterManager.Instance == null)
        {
            return;
        }

        CharacterRunTime character =
            CharacterManager.Instance.CurrentCharacter;

        if (character == null)
        {
            return;
        }

        // 캐릭터가 변경됐을 때 아이콘 변경
        if (currentCharacter != character)
        {
            currentCharacter = character;

            RefreshCharacter();
        }

        RefreshEnergy();
        RefreshSkillCoolDown();
        RefreshBurstCoolDown();
    }

    private void RefreshCharacter()
    {
        Sprite skillIcon = currentCharacter.Data.SkillIcon;

        skillCooldownBG.sprite = skillIcon;
        skillCoolDownFill.sprite = skillIcon;

        Sprite burstIcon = currentCharacter.Data.BurstIcon;

        burstCooldownBG.sprite = burstIcon;
        burstCoolDownFill.sprite = burstIcon;
    }

    private void RefreshEnergy()
    {
        float currentEnergy =
            currentCharacter.CurrentEnergy;

        float maxEnergy =
            currentCharacter.MaxEnergy;

        energyBarFill.fillAmount =
            maxEnergy > 0f
                ? currentEnergy / maxEnergy
                : 0f;

        currentEnergyText.text =
            Mathf.RoundToInt(currentEnergy).ToString();

        maxEnergyText.text =
            Mathf.RoundToInt(maxEnergy).ToString();
    }
    private void RefreshSkillCoolDown()
    {
        float remaining =
            currentCharacter.SkillCoolDown;

        float total =
            currentCharacter.Data.SkillCooldown;

        skillCoolDownFill.fillAmount =
            total > 0f
                ? 1f - (remaining / total)
                : 1f;

        if (remaining > 0f)
        {
            skillCoolDownText.gameObject.SetActive(true);
            skillCoolDownText.text = remaining.ToString("0.0");
        }
        else
        {
            skillCoolDownText.gameObject.SetActive(false);
        }
    }
    private void RefreshBurstCoolDown()
    {
        float remaining =
            currentCharacter.BurstCoolDown;

        float total =
            currentCharacter.Data.BurstCooldown;

        burstCoolDownFill.fillAmount =
            total > 0f
                ? 1f - (remaining / total)
                : 1f;

        if (remaining > 0f)
        {
            burstCoolDownText.gameObject.SetActive(true);
            burstCoolDownText.text = remaining.ToString("0.0");
        }
        else
        {
            burstCoolDownText.gameObject.SetActive(false);
        }
    }
}