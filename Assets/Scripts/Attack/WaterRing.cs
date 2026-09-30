using UnityEngine;

public class WaterRing : MonoBehaviour
{
    [Header("Skill")]
    [SerializeField] private float duration = 10f;
    [SerializeField] private float healInterval = 1f;

    private int healAmount;

    private float durationTimer;
    private float healTimer;

    public void Initialize(int healAmount)
    {
        this.healAmount = healAmount;

        durationTimer = duration;
        healTimer = healInterval;
    }

    private void Update()
    {
        FollowCharacter();
        UpdateDuration();
        UpdateHeal();
    }

    private void FollowCharacter()
    {
        transform.position =
            CharacterManager.Instance.CharacterView.CurrentView.transform.position;
    }

    private void UpdateDuration()
    {
        durationTimer -= Time.deltaTime;

        if (durationTimer <= 0f)
        {
            Destroy(gameObject);
        }
    }

    private void UpdateHeal()
    {
        healTimer -= Time.deltaTime;

        if (healTimer > 0f)
        {
            return;
        }

        CharacterRunTime currentCharacter =
            CharacterManager.Instance.CurrentCharacter;

        currentCharacter.Heal(healAmount);

        healTimer = healInterval;

        Debug.Log(
            $"WaterRing 회복 : {healAmount} / " +
            $"현재 HP : {currentCharacter.CurrentHP}"
        );
    }
}