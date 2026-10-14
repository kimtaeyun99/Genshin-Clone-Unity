using System.Collections.Generic;
using UnityEngine;

public class AttackProjectile : MonoBehaviour
{
    [Header("Projectile")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float lifeTime = 5f;
    [SerializeField] private LayerMask targetLayer;

    private Vector3 direction;
    private float damage;
    private ElementType elementType;

    private readonly HashSet<Health> hitTargets = new();

    public void Initialize(float damage, ElementType elementType, Vector3 direction)
    {
        this.damage = damage;
        this.elementType = elementType;
        // 방향의 길이가 투사체 속도에 영향을 주지 않도록 단위 벡터로 저장한다.
        this.direction = direction.normalized;

        hitTargets.Clear();
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        transform.position += direction * moveSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((targetLayer.value & (1 << other.gameObject.layer)) == 0)
            return;

        if (!other.TryGetComponent<Health>(out Health health))
            return;

        if (hitTargets.Contains(health))
            return;

        hitTargets.Add(health);
        health.TakeAttack(new AttackData(damage, elementType));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.forward * 3f);
    }
}
