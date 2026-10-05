using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IAttackable
{
    public event Action OnDead;
    public void TakeAttack(AttackData attackData)
    {
        CharacterRunTime currentCharacter = CharacterManager.Instance.CurrentCharacter;

        if (currentCharacter == null)
        {
            return;
        }

        currentCharacter.TakeDamage(attackData.Damage);

        if(currentCharacter.IsDead)
        {
            OnDead?.Invoke();
        }

        Debug.Log($"{currentCharacter.Data.CharacterName} 피격 : {attackData.Damage} / " + $"현재 HP : {currentCharacter.CurrentHP}"
        );
    }
}