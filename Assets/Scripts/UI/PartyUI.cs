using UnityEngine;

public class PartyUI : MonoBehaviour
{
    [SerializeField] private PartySlotUI[] partySlots;

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

            partySlots[i].SetSelected(
                i == party.CurrentIndex
            );
        }
    }
}