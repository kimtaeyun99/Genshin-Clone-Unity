using System;
using System.Collections.Generic;
using UnityEngine;

public class AttackProjectile : MonoBehaviour
{
    [Header("Projectile")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float lifeTime = 5f;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private Collider projectileCollider;

    private const int InitialOverlapCapacity = 16;

    private Vector3 direction;
    private float damage;
    private ElementType elementType;

    private Collider[] overlapResults = new Collider[InitialOverlapCapacity];
    private readonly HashSet<Health> hitTargets = new();

    public void Initialize(float damage, ElementType elementType, Vector3 direction)
    {
        this.damage = damage;
        this.elementType = elementType;
        this.direction = direction.normalized;

        hitTargets.Clear();
        Destroy(gameObject, lifeTime);

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

            if (count < overlapResults.Length)
                break;

            Array.Resize(ref overlapResults, overlapResults.Length * 2);
        }

        for (int i = 0; i < count; i++)
        {
            Collider other = overlapResults[i];
            if (other == null)
                continue;

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
}
