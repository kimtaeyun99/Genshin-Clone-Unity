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

    private CharacterManager characterManager;
    private CharacterRunTime currentCharacter;

    private int lastCurrentEnergyInt = int.MinValue;
    private int lastMaxEnergyInt = int.MinValue;
    private int lastSkillTenths = int.MinValue;
    private int lastBurstTenths = int.MinValue;

    private void OnEnable()
    {
        BindManager();
    }

    private void Start()
    {
        // OnEnable 시점에 CharacterManager.Awake가 아직 실행되지 않았을 때를 대비.
        BindManager();
    }

    private void OnDisable()
    {
        if (characterManager != null)
            characterManager.OnCurrentCharacterChanged -= HandleCharacterChanged;

        UnsubscribeCharacter();
        characterManager = null;
        currentCharacter = null;
    }

    private void BindManager()
    {
        CharacterManager manager = CharacterManager.Instance;
        if (manager == null)
            return;

        if (characterManager != manager)
        {
            if (characterManager != null)
                characterManager.OnCurrentCharacterChanged -= HandleCharacterChanged;

            characterManager = manager;
            characterManager.OnCurrentCharacterChanged += HandleCharacterChanged;
        }

        // UI가 나중에 활성화되더라도 현재 선택된 캐릭터를 표시.
        if (currentCharacter != manager.CurrentCharacter)
            HandleCharacterChanged(manager.CurrentCharacter);
    }

    private void HandleCharacterChanged(CharacterRunTime character)
    {
        UnsubscribeCharacter();
        currentCharacter = character;

        if (currentCharacter == null)
            return;

        currentCharacter.OnEnergyChanged += RefreshEnergy;
        currentCharacter.OnSkillCooldownChanged += RefreshSkillCoolDown;
        currentCharacter.OnBurstCooldownChanged += RefreshBurstCoolDown;

        // 새 캐릭터의 텍스트를 반드시 한 번 갱신.
        lastCurrentEnergyInt = int.MinValue;
        lastMaxEnergyInt = int.MinValue;
        lastSkillTenths = int.MinValue;
        lastBurstTenths = int.MinValue;

        RefreshCharacter();
        RefreshEnergy();
        RefreshSkillCoolDown();
        RefreshBurstCoolDown();
    }

    private void UnsubscribeCharacter()
    {
        if (currentCharacter == null)
            return;

        currentCharacter.OnEnergyChanged -= RefreshEnergy;
        currentCharacter.OnSkillCooldownChanged -= RefreshSkillCoolDown;
        currentCharacter.OnBurstCooldownChanged -= RefreshBurstCoolDown;
    }

    private void RefreshCharacter()
    {
        if (currentCharacter == null)
            return;

        Sprite skillIcon = currentCharacter.Data.SkillIcon;
        skillCooldownBG.sprite = skillIcon;
        skillCoolDownFill.sprite = skillIcon;

        Sprite burstIcon = currentCharacter.Data.BurstIcon;
        burstCooldownBG.sprite = burstIcon;
        burstCoolDownFill.sprite = burstIcon;
    }

    private void RefreshEnergy()
    {
        if (currentCharacter == null)
            return;

        float currentEnergy = currentCharacter.CurrentEnergy;
        float maxEnergy = currentCharacter.MaxEnergy;

        energyBarFill.fillAmount = maxEnergy > 0f
            ? currentEnergy / maxEnergy
            : 0f;

        // 눈에 보이는 정수 값이 바뀔 때만 텍스트를 다시 작성.
        int roundedCurrent = Mathf.RoundToInt(currentEnergy);
        int roundedMax = Mathf.RoundToInt(maxEnergy);
        if (lastCurrentEnergyInt != roundedCurrent)
        {
            currentEnergyText.text = roundedCurrent.ToString();
            lastCurrentEnergyInt = roundedCurrent;
        }
        if (lastMaxEnergyInt != roundedMax)
        {
            maxEnergyText.text = roundedMax.ToString();
            lastMaxEnergyInt = roundedMax;
        }
    }

    private void RefreshSkillCoolDown()
    {
        if (currentCharacter == null)
            return;

        float remaining = currentCharacter.SkillCoolDown;
        float total = currentCharacter.Data.SkillCooldown;

        skillCoolDownFill.fillAmount = total > 0f
            ? 1f - (remaining / total)
            : 1f;

        bool isCoolingDown = remaining > 0f;
        if (skillCoolDownText.gameObject.activeSelf != isCoolingDown)
            skillCoolDownText.gameObject.SetActive(isCoolingDown);

        if (!isCoolingDown)
            return;

        // 게이지는 매 틱 부드럽게 갱신하되, 소수 1자리 숫자가 바뀔 때만 문자열 갱신.
        int tenths = Mathf.RoundToInt(remaining * 10f);
        if (lastSkillTenths != tenths)
        {
            skillCoolDownText.text = remaining.ToString("0.0");
            lastSkillTenths = tenths;
        }
    }

    private void RefreshBurstCoolDown()
    {
        if (currentCharacter == null)
            return;

        float remaining = currentCharacter.BurstCoolDown;
        float total = currentCharacter.Data.BurstCooldown;

        burstCoolDownFill.fillAmount = total > 0f
            ? 1f - (remaining / total)
            : 1f;

        bool isCoolingDown = remaining > 0f;
        if (burstCoolDownText.gameObject.activeSelf != isCoolingDown)
            burstCoolDownText.gameObject.SetActive(isCoolingDown);

        if (!isCoolingDown)
            return;

        int tenths = Mathf.RoundToInt(remaining * 10f);
        if (lastBurstTenths != tenths)
        {
            burstCoolDownText.text = remaining.ToString("0.0");
            lastBurstTenths = tenths;
        }
    }
}
