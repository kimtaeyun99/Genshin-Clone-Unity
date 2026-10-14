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

    // WaterRing 인스턴스가 활성화된 동안 원소 탐지 결과 배열을 재사용한다.
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
    }
    private void UpdateElemental()
    {
        elementTimer -= Time.deltaTime;

        if (elementTimer > 0f)
        {
            return;
        }

        // NonAlloc 버퍼가 가득 찬 경우 일부 Collider가 누락됐을 수 있다.
        // 기존 OverlapSphere와 동일하게 모든 결과를 처리하기 위해 즉시 확장 및 재조회한다.
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
            if (collider.TryGetComponent<IElementApplicable>(out IElementApplicable elementApplicable))
            {
                elementApplicable.ApplyElement(ElementType.Hydro);
            }
        }

        elementTimer = elementInterval;
    }
}
