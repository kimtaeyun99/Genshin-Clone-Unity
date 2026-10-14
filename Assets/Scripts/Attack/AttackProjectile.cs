using System;
using System.Collections.Generic;
using UnityEngine;

public class AttackProjectile : MonoBehaviour
{
    [Header("Projectile")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float lifeTime = 5f;
    [SerializeField] private LayerMask targetLayer;

    private const int InitialOverlapCapacity = 16;

    private Vector3 direction;
    private float damage;
    private ElementType elementType;

    private Collider projectileCollider;
    private Collider[] overlapResults = new Collider[InitialOverlapCapacity];
    private readonly HashSet<Health> hitTargets = new();

    private void Awake()
    {
        projectileCollider = GetComponent<Collider>();

        if (projectileCollider is not BoxCollider &&
            projectileCollider is not CapsuleCollider)
        {
            Debug.LogError(
                "[AttackProjectile] BoxCollider 또는 CapsuleCollider가 필요합니다.", this);
        }
    }

    public void Initialize(float damage, ElementType elementType, Vector3 direction)
    {
        this.damage = damage;
        this.elementType = elementType;
        // 목표까지의 거리와 관계없이 일정한 이동 속도를 유지한다.
        this.direction = direction.normalized;

        hitTargets.Clear();
        Destroy(gameObject, lifeTime);

        // 생성될 때 이미 겹쳐 있는 대상도 첫 이동 전에 검사한다.
        CheckOverlaps();
    }

    private void Update()
    {
        Move();
        CheckOverlaps();
    }

    private void Move()
    {
        transform.position += direction * moveSpeed * Time.deltaTime;
    }

    private void CheckOverlaps()
    {
        if (projectileCollider == null || !projectileCollider.enabled)
            return;

        int count;
        while (true)
        {
            count = QueryOverlaps();

            // 꽉 찬 버퍼는 결과가 잘렸을 수 있으므로 확장해 재조회한다.
            if (count < overlapResults.Length)
                break;

            Array.Resize(ref overlapResults, overlapResults.Length * 2);
        }

        for (int i = 0; i < count; i++)
        {
            Collider other = overlapResults[i];
            if (other == null)
                continue;

            // 기존 Trigger 충돌과 마찬가지로 레이어 충돌 무시 규칙을 적용한다.
            if (Physics.GetIgnoreLayerCollision(gameObject.layer, other.gameObject.layer))
                continue;

            if (!other.TryGetComponent<Health>(out Health health))
                continue;

            if (!hitTargets.Add(health))
                continue;

            health.TakeAttack(new AttackData(damage, elementType));
        }
    }

    private int QueryOverlaps()
    {
        Vector3 scale = transform.lossyScale;
        float sx = Mathf.Abs(scale.x);
        float sy = Mathf.Abs(scale.y);
        float sz = Mathf.Abs(scale.z);

        if (projectileCollider is BoxCollider box)
        {
            Vector3 center = transform.TransformPoint(box.center);
            Vector3 halfExtents = Vector3.Scale(
                box.size * 0.5f, new Vector3(sx, sy, sz));

            return Physics.OverlapBoxNonAlloc(
                center,
                halfExtents,
                overlapResults,
                transform.rotation,
                targetLayer,
                QueryTriggerInteraction.Collide);
        }

        if (projectileCollider is CapsuleCollider capsule)
        {
            Vector3 axis;
            float axisScale;
            float radialScale;

            switch (capsule.direction)
            {
                case 0: // X
                    axis = transform.right;
                    axisScale = sx;
                    radialScale = Mathf.Max(sy, sz);
                    break;
                case 2: // Z
                    axis = transform.forward;
                    axisScale = sz;
                    radialScale = Mathf.Max(sx, sy);
                    break;
                default: // Y
                    axis = transform.up;
                    axisScale = sy;
                    radialScale = Mathf.Max(sx, sz);
                    break;
            }

            Vector3 center = transform.TransformPoint(capsule.center);
            float radius = capsule.radius * radialScale;
            float halfSegment = Mathf.Max(
                0f, capsule.height * axisScale * 0.5f - radius);
            Vector3 endOffset = axis * halfSegment;

            return Physics.OverlapCapsuleNonAlloc(
                center - endOffset,
                center + endOffset,
                radius,
                overlapResults,
                targetLayer,
                QueryTriggerInteraction.Collide);
        }

        return 0;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.forward * 3f);
    }
}
