using UnityEngine;

public class DamageUIManager : MonoBehaviour
{
    public static DamageUIManager Instance { get; private set; }

    [Header("Prefab")]
    [SerializeField] private DamageText damageTextPrefab;
    [SerializeField] private ReactionText reactionTextPrefab;

    [Header("Root")]
    [SerializeField] private RectTransform damageTextRoot;

    private Camera mainCamera;

    private void Awake()
    {
        Instance = this;

        mainCamera = Camera.main;
    }

    public void ShowDamage(
        float damage,
        ElementType elementType,
        Vector3 worldPosition)
    {
        Vector3 screenPosition =
            GetScreenPosition(worldPosition);

        if (screenPosition.z < 0f)
        {
            return;
        }

        DamageText damageText =
            Instantiate(
                damageTextPrefab,
                damageTextRoot
            );

        damageText.transform.position =
            screenPosition;

        damageText.Initialize(
            damage,
            elementType
        );
    }

    public void ShowReaction(
        ElementReactionType reaction,
        ElementType guestElement,
        Vector3 worldPosition)
    {
        if (reaction == ElementReactionType.None)
        {
            return;
        }

        Vector3 screenPosition =
            GetScreenPosition(worldPosition);

        if (screenPosition.z < 0f)
        {
            return;
        }

        ReactionText reactionText =
            Instantiate(
                reactionTextPrefab,
                damageTextRoot
            );

        reactionText.transform.position =
            screenPosition + Vector3.up * 40f;

        reactionText.Initialize(
            reaction,
            guestElement
        );
    }

    private Vector3 GetScreenPosition(
        Vector3 worldPosition)
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        return mainCamera.WorldToScreenPoint(
            worldPosition
        );
    }
}