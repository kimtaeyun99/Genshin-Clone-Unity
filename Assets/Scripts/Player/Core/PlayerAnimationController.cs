using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    // CharacterManager는 싱글톤이므로 Inspector에서 연결하지 않는다.
    // 이벤트를 안전하게 해제하기 위해 등록한 인스턴스만 기억한다.
    private CharacterManager subscribedManager;

    private Animator Animator => CharacterManager.Instance != null
        ? CharacterManager.Instance.CurrentAnimator
        : null;

    private static readonly int StateHash = Animator.StringToHash("State");
    private static readonly int JumpHash = Animator.StringToHash("Jump");
    private static readonly int DodgeHash = Animator.StringToHash("Dodge");
    private static readonly int NormalAttackHash = Animator.StringToHash("NormalAttack");
    private static readonly int NormalAttackIndexHash = Animator.StringToHash("NormalAttackIndex");
    private static readonly int SkillHash = Animator.StringToHash("Skill");
    private static readonly int SkillIndexHash = Animator.StringToHash("SkillIndex");
    private static readonly int BurstHash = Animator.StringToHash("Burst");
    private static readonly int DeadHash = Animator.StringToHash("Dead");

    private void OnEnable()
    {
        BindManager();
    }

    private void Start()
    {
        // OnEnable이 CharacterManager.Awake보다 먼저 호출될 수 있으므로 한 번 재시도.
        BindManager();
    }

    private void OnDisable()
    {
        if (subscribedManager != null)
            subscribedManager.OnCurrentCharacterChanged -= HandleCharacterChanged;

        subscribedManager = null;
    }

    private void BindManager()
    {
        CharacterManager manager = CharacterManager.Instance;
        if (manager == null || manager == subscribedManager)
            return;

        if (subscribedManager != null)
            subscribedManager.OnCurrentCharacterChanged -= HandleCharacterChanged;

        subscribedManager = manager;
        subscribedManager.OnCurrentCharacterChanged += HandleCharacterChanged;

        // UI/애니메이션 컨트롤러가 뒤늦게 활성화되어도 현재 캐릭터와 동기화.
        if (manager.CurrentCharacter != null)
            ResetAnimatorToIdle();
    }

    private void HandleCharacterChanged(CharacterRunTime character)
    {
        // CharacterManager.ChangeView가 완료된 뒤 호출되는 이벤트다.
        // 따라서 Animator 프로퍼티는 교체된 모델의 Animator를 가리킨다.
        ResetAnimatorToIdle();
    }

    public void ResetAnimatorToIdle()
    {
        if (CharacterManager.Instance == null)
            return;

        Animator animator = Animator;
        if (animator == null || animator.runtimeAnimatorController == null)
            return;

        // 기존 애니메이션 상태 / 트랜지션을 초기화한다.
        animator.Rebind();

        // Animator 파라미터의 정의와 해시는 이 클래스에서만 관리한다.
        animator.ResetTrigger(JumpHash);
        animator.ResetTrigger(DodgeHash);
        animator.ResetTrigger(NormalAttackHash);
        animator.ResetTrigger(SkillHash);
        animator.ResetTrigger(BurstHash);
        animator.ResetTrigger(DeadHash);
        animator.SetInteger(NormalAttackIndexHash, 0);
        animator.SetInteger(SkillIndexHash, 0);
        animator.SetInteger(StateHash, 0); // 기존 State 파라미터 규칙: 0 = Idle
        animator.Update(0f);
    }

    public void PlayIdle()
    {
        Animator.SetInteger(StateHash, 0);
    }

    public void PlayMove()
    {
        Animator.SetInteger(StateHash, 1);
    }

    public void PlayJump()
    {
        Animator.SetTrigger(JumpHash);
    }

    public void PlayDodge()
    {
        Animator.SetTrigger(DodgeHash);
    }

    public void PlayNormalAttack()
    {
        Animator.SetTrigger(NormalAttackHash);
    }

    public void SetNormalAttackIndex(int index)
    {
        Animator.SetInteger(NormalAttackIndexHash, index);
    }

    public void PlaySkill()
    {
        Animator.SetTrigger(SkillHash);
    }

    public void SetSkillIndex(int index)
    {
        Animator.SetInteger(SkillIndexHash, index);
    }

    public void PlayBurst()
    {
        Animator.SetTrigger(BurstHash);
    }

    public void PlayDead()
    {
        Animator.SetTrigger(DeadHash);
    }

    public AnimatorStateInfo GetCurrentStateInfo()
    {
        return Animator.GetCurrentAnimatorStateInfo(0);
    }

    public bool IsTransition()
    {
        return Animator.IsInTransition(0);
    }
}
