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

    public void SetCharacter(
        CharacterData data,
        CharacterRunTime runTime)
    {
        if (data == null)
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);

        characterIcon.sprite = data.Icon;
        characterNameText.text = data.CharacterName;

        if (runTime == null)
            return;

        healthBarFill.fillAmount = runTime.MaxHP > 0 ? (float)runTime.CurrentHP / runTime.MaxHP : 0f;
    }

    public void SetSelected(bool isSelected)
    {
        selectedEffect.SetActive(isSelected);
    }
}