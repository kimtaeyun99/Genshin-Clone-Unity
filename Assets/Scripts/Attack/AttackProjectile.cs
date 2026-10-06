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

    private readonly HashSet<IAttackable> hitTargets = new();

    public void Initialize(
        float damage,
        ElementType elementType,
        Vector3 direction)
    {
        this.damage = damage;
        this.elementType = elementType;
        this.direction = direction;

        hitTargets.Clear();

        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        transform.position +=
            direction * moveSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((targetLayer.value & (1 << other.gameObject.layer)) == 0)
        {
            return;
        }
        if (!other.TryGetComponent<IAttackable>(out IAttackable attackable))
        {
            return;
        }

        if (hitTargets.Contains(attackable))
        {
            return;
        }

        hitTargets.Add(attackable);

        AttackData attackData =
            new AttackData(damage, elementType);

        attackable.TakeAttack(attackData);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        Gizmos.DrawRay(
            transform.position,
            transform.forward * 3f
        );
    }
}