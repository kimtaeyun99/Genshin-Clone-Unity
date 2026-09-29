using UnityEngine;

public class CharacterView : MonoBehaviour
{
    [Header("Model")]
    [SerializeField] private Transform modelRoot;

    private GameObject currentView;
    private Animator currentAnimator;
    private AttackHitbox currentAttackHitbox;

    public Animator CurrentAnimator => currentAnimator;
    public AttackHitbox CurrentAttackHitbox => currentAttackHitbox;

    public void ChangeView(CharacterData data)
    {
        if (data == null || data.Prefab == null)
        {
            return;
        }

        if (currentView != null)
        {
            Destroy(currentView);
        }

        currentView = Instantiate(data.Prefab, modelRoot);

        currentView.transform.localPosition = Vector3.zero;
        currentView.transform.localRotation = Quaternion.identity;
        currentView.transform.localScale = Vector3.one;


        currentAttackHitbox = currentView.GetComponentInChildren<AttackHitbox>(true);

        currentAnimator = currentView.GetComponentInChildren<Animator>();

        
        if (currentAnimator == null)
        {
            currentAnimator = currentView.AddComponent<Animator>();
        }

        currentAnimator.avatar = data.Avatar;
        currentAnimator.runtimeAnimatorController = data.AnimatorController;
    }
}