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

    private CharacterManager characterManager;
    private CharacterRunTime currentCharacter;

    private int previousCurrentHP = int.MinValue;
    private int previousMaxHP = int.MinValue;

    private void OnEnable()
    {
        BindManager();
    }

    private void Start()
    {
        if (characterManager == null)
            BindManager();
    }

    private void OnDisable()
    {
        if (characterManager != null)
            characterManager.OnCurrentCharacterChanged -= HandleCharacterChanged;

        UnsubscribeCharacter();
        currentCharacter = null;
        characterManager = null;
    }

    private void BindManager()
    {
        CharacterManager manager = CharacterManager.Instance;
        if (manager == null || characterManager == manager)
            return;

        if (characterManager != null)
            characterManager.OnCurrentCharacterChanged -= HandleCharacterChanged;

        UnsubscribeCharacter();
        currentCharacter = null;

        characterManager = manager;
        characterManager.OnCurrentCharacterChanged += HandleCharacterChanged;

        HandleCharacterChanged(characterManager.CurrentCharacter);
    }

    private void HandleCharacterChanged(CharacterRunTime character)
    {
        UnsubscribeCharacter();
        currentCharacter = character;
        previousCurrentHP = int.MinValue;
        previousMaxHP = int.MinValue;

        if (currentCharacter == null)
            return;

        currentCharacter.OnHealthChanged += RefreshHealthUI;
        RefreshHealthUI(currentCharacter.CurrentHP, currentCharacter.MaxHP);
    }

    private void UnsubscribeCharacter()
    {
        if (currentCharacter != null)
            currentCharacter.OnHealthChanged -= RefreshHealthUI;
    }

    private void RefreshHealthUI(int currentHP, int maxHP)
    {
        healthBarFill.fillAmount = maxHP > 0
            ? (float)currentHP / maxHP
            : 0f;

        if (previousCurrentHP != currentHP)
        {
            currentHPText.text = currentHP.ToString();
            previousCurrentHP = currentHP;
        }

        if (previousMaxHP != maxHP)
        {
            maxHPText.text = maxHP.ToString();
            previousMaxHP = maxHP;
        }
    }
}
