using UnityEngine;

public class CharacterView : MonoBehaviour
{
    [Header("Model")]
    [SerializeField] private Transform modelRoot;

    [Header("Animation")]
    [SerializeField] private PlayerAnimation playerAnimation;
    private GameObject currentView;

    public void ChangeView(CharacterData data)
    {
        if (data == null || data.Prefab == null)
        {
            return;
        }

        // 기존 캐릭터 View 제거
        if (currentView != null)
        {
            Destroy(currentView);
        }

        // 새로운 캐릭터 View 생성
        currentView = Instantiate(data.Prefab,modelRoot);

        currentView.transform.localPosition = Vector3.zero;
        currentView.transform.localRotation = Quaternion.identity;
        currentView.transform.localScale = Vector3.one;

        Animator animator = currentView.GetComponentInChildren<Animator>();

        if (animator != null)
        {
            animator.runtimeAnimatorController = data.AnimatorController;

            playerAnimation.SetAnimator(animator);
        }
    }
}