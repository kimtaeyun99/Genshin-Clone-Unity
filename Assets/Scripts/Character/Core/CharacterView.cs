using UnityEngine;

public class CharacterView : MonoBehaviour
{
    [Header("Model")]
    [SerializeField] private Transform modelRoot;

    private GameObject currentView;
    private Animator currentAnimator;

    public Animator CurrentAnimator => currentAnimator;

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

        // 생성된 캐릭터에 Animator가 있는지 먼저 찾기
        currentAnimator = currentView.GetComponentInChildren<Animator>();

        // 없으면 추가
        if (currentAnimator == null)
        {
            currentAnimator = currentView.AddComponent<Animator>();
        }

        currentAnimator.avatar = data.Avatar;
        currentAnimator.runtimeAnimatorController = data.AnimatorController;
    }
}