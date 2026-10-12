using UnityEngine;

public class PartyUI : MonoBehaviour
{
    [SerializeField] private PartySlotUI[] partySlots;

    [Header("Player")]
    [SerializeField] private PlayerController player;

    private void Update()
    {
        RefreshPartyUI();
    }

    private void RefreshPartyUI()
    {
        CharacterParty party = CharacterParty.Instance;
        CharacterManager manager = CharacterManager.Instance;

        if (party == null || manager == null)
            return;

        for (int i = 0; i < partySlots.Length; i++)
        {
            if (i >= party.Count)
            {
                partySlots[i].gameObject.SetActive(false);
                continue;
            }

            CharacterData data = party.GetCharacterData(i);

            CharacterRunTime runTime =
                manager.GetCharacter(data);

            partySlots[i].SetCharacter(data, runTime);

            bool isSelected =
                i == party.CurrentIndex;

            partySlots[i].SetSelected(isSelected);

            // 현재 캐릭터에는 교체 쿨타임 표시 X
            if (isSelected)
            {
                partySlots[i].SetSwitchCoolDown(0f);
            }
            else
            {
                partySlots[i].SetSwitchCoolDown(
                    player.SwitchCoolDownTimer
                );
            }
        }
    }
}