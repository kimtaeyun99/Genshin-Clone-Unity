using UnityEngine;

public class PartyUI : MonoBehaviour
{
    [SerializeField] private PartySlotUI[] partySlots;

    [Header("Player")]
    [SerializeField] private PlayerController player;

    private CharacterParty party;
    private CharacterManager manager;
    private CharacterRunTime[] slotRuntimes;

    private void Awake()
    {
        slotRuntimes = new CharacterRunTime[partySlots.Length];
    }

    private void OnEnable()
    {
        BindEvents();
    }

    private void Start()
    {
        if (party == null)
            BindEvents();
    }

    private void OnDisable()
    {
        if (party != null)
        {
            party.OnPartyChanged -= RefreshPartySlots;
            party.OnSelectionChanged -= HandleSelectionChanged;
        }

        if (player != null)
            player.OnSwitchCooldownChanged -= RefreshSwitchCooldown;

        UnsubscribeHealthEvents();
        party = null;
        manager = null;
    }

    private void BindEvents()
    {
        if (party != null)
            return;

        CharacterParty currentParty = CharacterParty.Instance;
        CharacterManager currentManager = CharacterManager.Instance;

        if (currentParty == null || currentManager == null || player == null)
            return;

        party = currentParty;
        manager = currentManager;

        party.OnPartyChanged += RefreshPartySlots;
        party.OnSelectionChanged += HandleSelectionChanged;
        player.OnSwitchCooldownChanged += RefreshSwitchCooldown;

        RefreshPartySlots();
    }

    private void RefreshPartySlots()
    {
        if (party == null || manager == null)
            return;

        UnsubscribeHealthEvents();

        for (int i = 0; i < partySlots.Length; i++)
        {
            PartySlotUI slot = partySlots[i];
            if (slot == null)
                continue;

            if (i >= party.Count)
            {
                slot.SetCharacter(null, null);
                continue;
            }

            CharacterData data = party.GetCharacterData(i);
            CharacterRunTime runtime = data != null ? manager.GetCharacter(data) : null;

            slot.SetCharacter(data, runtime);
            slotRuntimes[i] = runtime;

            if (runtime != null)
                runtime.OnHealthChanged += HandlePartyHealthChanged;
        }

        RefreshSelection();
        RefreshSwitchCooldown(player.SwitchCoolDownTimer, player.SwitchCoolDown);
    }

    private void HandleSelectionChanged()
    {
        RefreshSelection();
        RefreshSwitchCooldown(player.SwitchCoolDownTimer, player.SwitchCoolDown);
    }

    private void RefreshSelection()
    {
        if (party == null)
            return;

        for (int i = 0; i < partySlots.Length; i++)
        {
            PartySlotUI slot = partySlots[i];
            if (slot == null || i >= party.Count)
                continue;

            slot.SetSelected(i == party.CurrentIndex);
        }
    }

    private void RefreshSwitchCooldown(float remaining, float total)
    {
        if (party == null)
            return;

        for (int i = 0; i < partySlots.Length; i++)
        {
            PartySlotUI slot = partySlots[i];
            if (slot == null || i >= party.Count)
                continue;

            float displayRemaining = i == party.CurrentIndex ? 0f : remaining;
            slot.SetSwitchCoolDown(displayRemaining, total);
        }
    }

    private void HandlePartyHealthChanged(int currentHP, int maxHP)
    {
        for (int i = 0; i < slotRuntimes.Length; i++)
        {
            if (slotRuntimes[i] == null || partySlots[i] == null)
                continue;

            partySlots[i].SetHealth(slotRuntimes[i].CurrentHP, slotRuntimes[i].MaxHP);
        }
    }

    private void UnsubscribeHealthEvents()
    {
        if (slotRuntimes == null)
            return;

        for (int i = 0; i < slotRuntimes.Length; i++)
        {
            CharacterRunTime runtime = slotRuntimes[i];
            if (runtime == null)
                continue;

            runtime.OnHealthChanged -= HandlePartyHealthChanged;
            slotRuntimes[i] = null;
        }
    }
}
