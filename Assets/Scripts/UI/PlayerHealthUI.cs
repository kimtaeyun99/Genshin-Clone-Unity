using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    [Header("Health Bar")]
    [SerializeField] private Image healthBarFill;

    [Header("Health Text")]
    [SerializeField] private TMP_Text currentHPText;
    [SerializeField] private TMP_Text maxHPText;

    private CharacterRunTime currentCharacter;

    private int previousCurrentHP = -1;
    private int previousMaxHP = -1;

    private void Update()
    {
        CharacterRunTime character =
            CharacterManager.Instance.CurrentCharacter;

        if (character == null)
            return;

        if (currentCharacter != character)
        {
            currentCharacter = character;

            previousCurrentHP = -1;
            previousMaxHP = -1;
        }

        if (previousCurrentHP == currentCharacter.CurrentHP &&
            previousMaxHP == currentCharacter.MaxHP)
            return;

        RefreshHealthUI();
    }

    private void RefreshHealthUI()
    {
        int currentHP = currentCharacter.CurrentHP;
        int maxHP = currentCharacter.MaxHP;

        healthBarFill.fillAmount =
            maxHP > 0 ? (float)currentHP / maxHP : 0f;

        currentHPText.text = currentHP.ToString();
        maxHPText.text = maxHP.ToString();

        previousCurrentHP = currentHP;
        previousMaxHP = maxHP;
    }
}