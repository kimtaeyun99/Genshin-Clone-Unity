using UnityEngine;

public class WaterRing : MonoBehaviour
{
    [Header("Skill")]
    [SerializeField] private float duration = 10f;
    [SerializeField] private float healInterval = 1f;

    private float healPercent;
    private float durationTimer;
    private float healTimer;

    public void Initialize(float healPercent)
    {
        this.healPercent = healPercent;

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
        transform.position = CharacterManager.Instance.CharacterView.CurrentView.transform.position;
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

        CharacterRunTime currentCharacter = CharacterManager.Instance.CurrentCharacter;

        int healAmount = Mathf.RoundToInt(
            currentCharacter.MaxHP * (healPercent / 100f)
        );

        currentCharacter.Heal(healAmount);

        healTimer = healInterval;

        Debug.Log(
            $"WaterRing 회복 : {healAmount} / " +
            $"현재 HP : {currentCharacter.CurrentHP}"
        );
    }
}