using System.Collections.Generic;
using UnityEngine;

public class AttackProjectile : MonoBehaviour
{
    [Header("Projectile")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float lifeTime = 5f;
    [SerializeField] private LayerMask targetLayer;

    private float damage;
    private ElementType elementType;

    private readonly HashSet<IAttackable> hitTargets = new();

    public void Initialize(
        float damage,
        ElementType elementType)
    {
        this.damage = damage;
        this.elementType = elementType;

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
            transform.forward * moveSpeed * Time.deltaTime;
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
}