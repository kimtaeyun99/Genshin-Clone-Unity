using UnityEngine;
using UnityEngine.EventSystems;

public class StartMenuButton :
    MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler
{
    [Header("Image")]
    [SerializeField] private GameObject normalImage;
    [SerializeField] private GameObject hoverImage;

    [Header("Hover")]
    [SerializeField] private GameObject hoverDecor;

    [Header("Scale")]
    [SerializeField] private float hoverScale = 1.1f;
    [SerializeField] private float scaleSpeed = 10f;

    private Vector3 targetScale;
    private Vector3 originalScale;

    private void Awake()
    {
        originalScale = transform.localScale;
        targetScale = originalScale;

        normalImage.SetActive(true);
        hoverImage.SetActive(false);
        hoverDecor.SetActive(false);
    }

    private void Update()
    {
        transform.localScale =
            Vector3.Lerp(
                transform.localScale,
                targetScale,
                scaleSpeed * Time.unscaledDeltaTime
            );
    }

    public void OnPointerEnter(
        PointerEventData eventData)
    {
        normalImage.SetActive(false);
        hoverImage.SetActive(true);
        hoverDecor.SetActive(true);

        targetScale =
            originalScale * hoverScale;
    }

    public void OnPointerExit(
        PointerEventData eventData)
    {
        normalImage.SetActive(true);
        hoverImage.SetActive(false);
        hoverDecor.SetActive(false);

        targetScale =
            originalScale;
    }
}