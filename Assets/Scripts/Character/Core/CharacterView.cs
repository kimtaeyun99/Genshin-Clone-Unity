using System.Collections.Generic;
using UnityEngine;

public class CharacterView : MonoBehaviour
{
    [Header("Model")]
    [SerializeField] private Transform modelRoot;

    private readonly Dictionary<CharacterData, CachedView> cachedViews = new();
    private CachedView current;

    public GameObject CurrentView => current?.Instance;
    public Animator CurrentAnimator => current?.Animator;
    public AttackHitbox CurrentAttackHitbox => current?.AttackHitbox;
    public ICharacterAttack CurrentAttack => current?.Attack;
    public ICharacterSkill CurrentSkill => current?.Skill;
    public ICharacterBurst CurrentBurst => current?.Burst;

    public void ChangeView(CharacterData data)
    {
        if (data == null || data.Prefab == null || modelRoot == null)
            return;

        if (!cachedViews.TryGetValue(data, out CachedView next) || next.Instance == null)
        {
            next = CreateCachedView(data);
            cachedViews[data] = next;
        }

        if (current != null && current != next && current.Instance != null)
            current.Instance.SetActive(false);

        current = next;
        ResetLocalTransform(next.Instance.transform);

        if (!next.Instance.activeSelf)
            next.Instance.SetActive(true);
    }

    private CachedView CreateCachedView(CharacterData data)
    {
        GameObject model = Instantiate(data.Prefab, modelRoot);
        ResetLocalTransform(model.transform);

        Animator animator = model.GetComponentInChildren<Animator>(true);
        if (animator == null)
            animator = model.AddComponent<Animator>();

        animator.avatar = data.Avatar;
        animator.runtimeAnimatorController = data.AnimatorController;

        return new CachedView(
            model,
            animator,
            model.GetComponentInChildren<AttackHitbox>(true),
            model.GetComponentInChildren<ICharacterAttack>(true),
            model.GetComponentInChildren<ICharacterSkill>(true),
            model.GetComponentInChildren<ICharacterBurst>(true)
        );
    }

    private static void ResetLocalTransform(Transform model)
    {
        model.localPosition = Vector3.zero;
        model.localRotation = Quaternion.identity;
        model.localScale = Vector3.one;
    }

    private sealed class CachedView
    {
        public readonly GameObject Instance;
        public readonly Animator Animator;
        public readonly AttackHitbox AttackHitbox;
        public readonly ICharacterAttack Attack;
        public readonly ICharacterSkill Skill;
        public readonly ICharacterBurst Burst;

        public CachedView(
            GameObject instance,
            Animator animator,
            AttackHitbox attackHitbox,
            ICharacterAttack attack,
            ICharacterSkill skill,
            ICharacterBurst burst)
        {
            Instance = instance;
            Animator = animator;
            AttackHitbox = attackHitbox;
            Attack = attack;
            Skill = skill;
            Burst = burst;
        }
    }
}
