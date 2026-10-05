using UnityEngine;

public class WaterRing : MonoBehaviour
{
    [Header("Skill")]
    [SerializeField] private float duration = 10f;
    [SerializeField] private float healInterval = 1f;

    [Header("Element")]
    [SerializeField] private float elementRadius = 3f;
    [SerializeField] private float elementInterval = 1f;

    private float elementTimer;

    private int healAmount;

    private float durationTimer;
    private float healTimer;

    public void Initialize(int healAmount)
    {
        this.healAmount = healAmount;

        elementTimer = 0f;
        durationTimer = duration;
        healTimer = healInterval;
    }

    private void Update()
    {
        FollowCharacter();
        UpdateDuration();
        UpdateHeal();
        UpdateElemental();
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
    private void UpdateElemental()
    {
        elementTimer -= Time.deltaTime;

        if(elementTimer > 0f)
        {
            return;
        }

        Collider[] colliders = Physics.OverlapSphere(transform.position, elementRadius);

        foreach(Collider collider in colliders)
        {
            if(collider.TryGetComponent<IElementApplicable>(out IElementApplicable elementApplicable))
            {
                elementApplicable.ApplyElement(ElementType.Hydro);
            }
        }

        elementTimer = elementInterval;
    }
}