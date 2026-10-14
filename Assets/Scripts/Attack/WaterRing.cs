using System;
using UnityEngine;

public class WaterRing : MonoBehaviour
{
    [Header("Skill")]
    [SerializeField] private float duration = 10f;
    [SerializeField] private float healInterval = 1f;

    [Header("Element")]
    [SerializeField] private float elementRadius = 3f;
    [SerializeField] private float elementInterval = 1f;

    private const int InitialElementBufferSize = 16;
    private Collider[] elementColliders = new Collider[InitialElementBufferSize];

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

        if (UpdateDuration())
            return;

        UpdateHeal();
        UpdateElemental();
    }

    private void FollowCharacter()
    {
        transform.position =
            CharacterManager.Instance.CharacterView.CurrentView.transform.position;
    }

    private bool UpdateDuration()
    {
        durationTimer -= Time.deltaTime;
        if (durationTimer > 0f)
            return false;

        Destroy(gameObject);
        return true;
    }

    private void UpdateHeal()
    {
        healTimer -= Time.deltaTime;
        if (healTimer > 0f)
            return;

        CharacterRunTime currentCharacter = CharacterManager.Instance.CurrentCharacter;
        currentCharacter.Heal(healAmount);
        healTimer = healInterval;
    }

    private void UpdateElemental()
    {
        elementTimer -= Time.deltaTime;
        if (elementTimer > 0f)
            return;

        int hitCount;
        while (true)
        {
            hitCount = Physics.OverlapSphereNonAlloc(
                transform.position,
                elementRadius,
                elementColliders
            );

            if (hitCount < elementColliders.Length)
                break;

            Array.Resize(ref elementColliders, elementColliders.Length * 2);
        }

        for (int i = 0; i < hitCount; i++)
        {
            Collider collider = elementColliders[i];
            if (collider.TryGetComponent<Health>(out Health health))
                health.ApplyElement(ElementType.Hydro);
        }

        elementTimer = elementInterval;
    }
}
