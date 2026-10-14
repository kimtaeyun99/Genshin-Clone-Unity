using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class AttackHitbox : MonoBehaviour
{
    [SerializeField] private LayerMask targetLayer;

    private const int InitialOverlapCapacity = 16;

    private float damage;
    private ElementType element;

    private BoxCollider hitboxCollider;
    private Collider[] overlapResults = new Collider[InitialOverlapCapacity];

    private readonly HashSet<Health> hitTargets = new();
    private HashSet<Health> sharedHitTargets;

    public float Damage => damage;
    public ElementType Element => element;
    public event Action<Health> OnHit;

    private void Awake()
    {
        hitboxCollider = GetComponent<BoxCollider>();
    }

    private void OnEnable()
    {
        // 공격 애니메이션이 히트박스를 켠 순간, 이미 겹쳐 있는 대상도 탐지한다.
        CheckOverlaps();
    }

    private void FixedUpdate()
    {
        // 활성 공격 판정만 주기적으로 조회한다. 같은 공격의 중복 피격은 HashSet이 막는다.
        CheckOverlaps();
    }

    public void SetAttack(float atk, float multiplier, ElementType elementType)
    {
        damage = atk * multiplier;
        element = elementType;
    }

    public void SetSharedHitTargets(HashSet<Health> sharedHitTargets)
    {
        this.sharedHitTargets = sharedHitTargets;
    }

    public void ClearSharedHitTargets()
    {
        sharedHitTargets = null;
    }

    public void ClearHitbox()
    {
        hitTargets.Clear();
    }

    private void CheckOverlaps()
    {
        if (hitboxCollider == null || !hitboxCollider.enabled)
            return;

        // 기존 BoxCollider의 로컬 중심/크기/회전을 월드 좌표의 OverlapBox로 옮긴다.
        Vector3 center = transform.TransformPoint(hitboxCollider.center);
        Vector3 scale = transform.lossyScale;
        Vector3 halfExtents = Vector3.Scale(
            hitboxCollider.size * 0.5f,
            new Vector3(Mathf.Abs(scale.x), Mathf.Abs(scale.y), Mathf.Abs(scale.z))
        );

        int count;
        while (true)
        {
            count = Physics.OverlapBoxNonAlloc(
                center,
                halfExtents,
                overlapResults,
                transform.rotation,
                targetLayer,
                QueryTriggerInteraction.Collide
            );

            // 결과 버퍼가 꽉 찼다면 누락 가능성이 있으므로 확장해서 재탐지한다.
            if (count < overlapResults.Length)
                break;

            Array.Resize(ref overlapResults, overlapResults.Length * 2);
        }

        HashSet<Health> targets = sharedHitTargets ?? hitTargets;
        for (int i = 0; i < count; i++)
        {
            Collider other = overlapResults[i];
            if (other == null)
                continue;

            // OnTriggerStay와 동일하게 Physics 레이어 간 충돌 무시 설정을 존중한다.
            if (Physics.GetIgnoreLayerCollision(gameObject.layer, other.gameObject.layer))
                continue;

            if (!other.TryGetComponent<Health>(out Health health))
                continue;

            if (!targets.Add(health))
                continue;

            health.TakeAttack(new AttackData(damage, element));
            OnHit?.Invoke(health);
        }
    }
}
