using UnityEngine;

public class DamageUIManager : MonoBehaviour
{
    public static DamageUIManager Instance { get; private set; }

    [Header("Damage UI")]
    [SerializeField] private DamageText damageTextPrefab;
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
        ElementReactionType reaction,
        Vector3 worldPosition)
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        Vector3 screenPosition =
            mainCamera.WorldToScreenPoint(
                worldPosition
            );

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
            elementType,
            reaction
        );
    }
}