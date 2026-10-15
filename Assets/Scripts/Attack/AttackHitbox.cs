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

    [SerializeField] private BoxCollider hitboxCollider;
    private Collider[] overlapResults = new Collider[InitialOverlapCapacity];

    private readonly HashSet<Health> hitTargets = new();
    private HashSet<Health> sharedHitTargets;

    public float Damage => damage;
    public ElementType Element => element;
    public event Action OnHit;

    private void OnEnable()
    {
        CheckOverlaps();
    }

    private void FixedUpdate()
    {
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

        Vector3 center = transform.TransformPoint(hitboxCollider.center);
        Vector3 scale = transform.lossyScale;
        Vector3 halfExtents = Vector3.Scale(hitboxCollider.size * 0.5f,new Vector3(Mathf.Abs(scale.x), Mathf.Abs(scale.y), Mathf.Abs(scale.z)));

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

            if (Physics.GetIgnoreLayerCollision(gameObject.layer, other.gameObject.layer))
                continue;

            if (!other.TryGetComponent<Health>(out Health health))
                continue;

            if (!targets.Add(health))
                continue;

            health.TakeAttack(new AttackData(damage, element));
            OnHit?.Invoke();
        }
    }
}
