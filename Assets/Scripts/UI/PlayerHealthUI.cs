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

    // CharacterManager는 싱글톤이므로 Inspector 연결하지 않음.
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
        // OnEnable이 CharacterManager.Awake보다 먼저 실행된 경우 한 번 재시도.
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

        // 관리자 인스턴스가 바뀔 가능성에 대비한 방어적 해제.
        if (characterManager != null)
            characterManager.OnCurrentCharacterChanged -= HandleCharacterChanged;

        UnsubscribeCharacter();
        currentCharacter = null;

        characterManager = manager;
        characterManager.OnCurrentCharacterChanged += HandleCharacterChanged;

        // UI 활성화 전에 캐릭터가 선택되어 있어도 초기 체력을 표시.
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

        // HP 표시값이 실제로 달라질 때만 텍스트 갱신.
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
