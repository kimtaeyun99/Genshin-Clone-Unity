using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombatUI : MonoBehaviour
{
    [Header("Energy")]
    [SerializeField] private Image energyBarFill;
    [SerializeField] private TMP_Text currentEnergyText;
    [SerializeField] private TMP_Text maxEnergyText;

    [Header("Cooldown")]
    [SerializeField] private CooldownUI skillCooldownUI;
    [SerializeField] private CooldownUI burstCooldownUI;

    private CharacterManager characterManager;
    private CharacterRunTime currentCharacter;

    private int lastCurrentEnergy = int.MinValue;
    private int lastMaxEnergy = int.MinValue;

    private void OnEnable()
    {
        BindManager();
    }

    private void Start()
    {
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
        currentCharacter.OnSkillCoolDownChanged += RefreshSkillCoolDown;
        currentCharacter.OnBurstCoolDownChanged += RefreshBurstCoolDown;

        lastCurrentEnergy = int.MinValue;
        lastMaxEnergy = int.MinValue;

        skillCooldownUI.ResetCache();
        burstCooldownUI.ResetCache();

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
        currentCharacter.OnSkillCoolDownChanged -= RefreshSkillCoolDown;
        currentCharacter.OnBurstCoolDownChanged -= RefreshBurstCoolDown;
    }

    private void RefreshCharacter()
    {
        if (currentCharacter == null)
            return;

        skillCooldownUI.SetIcon(currentCharacter.Data.SkillIcon);
        burstCooldownUI.SetIcon(currentCharacter.Data.BurstIcon);
    }

    private void RefreshEnergy()
    {
        if (currentCharacter == null)
            return;

        int currentEnergy = currentCharacter.CurrentEnergy;
        int maxEnergy = currentCharacter.BurstEnergyCost;

        energyBarFill.fillAmount = maxEnergy > 0
            ? (float)currentEnergy / maxEnergy
            : 0f;

        if (lastCurrentEnergy != currentEnergy)
        {
            currentEnergyText.text = currentEnergy.ToString();
            lastCurrentEnergy = currentEnergy;
        }

        if (lastMaxEnergy != maxEnergy)
        {
            maxEnergyText.text = maxEnergy.ToString();
            lastMaxEnergy = maxEnergy;
        }
    }

    private void RefreshSkillCoolDown()
    {
        if (currentCharacter == null)
            return;

        skillCooldownUI.SetCooldown(
            currentCharacter.SkillCoolDown,
            currentCharacter.Data.SkillCoolDown
        );
    }

    private void RefreshBurstCoolDown()
    {
        if (currentCharacter == null)
            return;

        burstCooldownUI.SetCooldown(
            currentCharacter.BurstCoolDown,
            currentCharacter.Data.BurstCoolDown
        );
    }
}
