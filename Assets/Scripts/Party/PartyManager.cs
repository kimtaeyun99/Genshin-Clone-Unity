using System;
using UnityEngine;

public class PartyManager : MonoBehaviour
{
    private const int MaxPartySize = 4;

    [Header("Reference")]
    [SerializeField] private CharacterRoster roster;

    [Header("Party")]
    [SerializeField]
    private CharacterRuntime[] party =
        new CharacterRuntime[MaxPartySize];

    [Header("Active Character")]
    [SerializeField] private int activeSlotIndex = 0;

    public CharacterRuntime ActiveCharacter
    {
        get
        {
            if (!IsValidSlot(activeSlotIndex))
            {
                return null;
            }

            return party[activeSlotIndex];
        }
    }

    public int ActiveSlotIndex => activeSlotIndex;

    public event Action<CharacterRuntime> OnActiveCharacterChanged;


    private void Start()
    {
        InitializeParty();
    }


    // 현재 보유 캐릭터 중 앞의 4명을 테스트용 파티로 등록
    private void InitializeParty()
    {
        if (roster == null)
        {
            Debug.LogError("CharacterRoster가 연결되어 있지 않습니다.");
            return;
        }

        int count = Mathf.Min(
            roster.OwnedCharacters.Count,
            MaxPartySize
        );

        for (int i = 0; i < count; i++)
        {
            party[i] = roster.OwnedCharacters[i];
        }

        activeSlotIndex = 0;
    }


    // 해당 슬롯의 파티원 가져오기
    public CharacterRuntime GetPartyMember(int slotIndex)
    {
        if (!IsValidSlot(slotIndex))
        {
            return null;
        }

        return party[slotIndex];
    }


    // 파티원 등록 / 교체
    public bool SetPartyMember(
        int slotIndex,
        CharacterRuntime character)
    {
        if (!IsValidSlot(slotIndex))
        {
            return false;
        }

        if (character == null)
        {
            return false;
        }

        // 다른 슬롯에 이미 들어있는 캐릭터인지 확인
        if (ContainsOtherSlot(character, slotIndex))
        {
            Debug.LogWarning(
                $"{character.Data.CharacterName}은 이미 파티에 있습니다."
            );

            return false;
        }

        party[slotIndex] = character;

        return true;
    }


    // 현재 조작 캐릭터 변경
    public bool SwitchCharacter(int slotIndex)
    {
        if (!IsValidSlot(slotIndex))
        {
            return false;
        }

        if (party[slotIndex] == null)
        {
            return false;
        }

        if (activeSlotIndex == slotIndex)
        {
            return false;
        }

        activeSlotIndex = slotIndex;

        OnActiveCharacterChanged?.Invoke(
            ActiveCharacter
        );

        return true;
    }


    private bool ContainsOtherSlot(
        CharacterRuntime character,
        int excludeSlotIndex)
    {
        for (int i = 0; i < party.Length; i++)
        {
            if (i == excludeSlotIndex)
            {
                continue;
            }

            if (party[i] == character)
            {
                return true;
            }
        }

        return false;
    }


    private bool IsValidSlot(int slotIndex)
    {
        return slotIndex >= 0 &&
               slotIndex < MaxPartySize;
    }
}