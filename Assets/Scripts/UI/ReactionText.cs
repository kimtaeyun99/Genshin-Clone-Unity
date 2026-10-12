using TMPro;
using UnityEngine;

public class ReactionText : MonoBehaviour
{
    [Header("Text")]
    [SerializeField] private TMP_Text reactionText;

    [Header("Animation")]
    [SerializeField] private float moveSpeed = 50f;
    [SerializeField] private float duration = 1f;

    [Header("Color")]
    [SerializeField]
    private Color pyroColor =
        new Color32(255, 80, 60, 255);

    [SerializeField]
    private Color hydroColor =
        new Color32(60, 150, 255, 255);

    [SerializeField]
    private Color cryoColor =
        new Color32(120, 220, 255, 255);

    [SerializeField]
    private Color electroColor =
        new Color32(190, 90, 255, 255);

    private float timer;

    public void Initialize(
        ElementReactionType reaction,
        ElementType guestElement)
    {
        reactionText.text =
            GetReactionName(reaction);

        reactionText.color =
            GetElementColor(guestElement);

        timer = duration;
    }

    private void Update()
    {
        transform.position +=
            Vector3.up * moveSpeed * Time.deltaTime;

        timer -= Time.deltaTime;

        float alpha =
            Mathf.Clamp01(timer / duration);

        Color color = reactionText.color;
        color.a = alpha;
        reactionText.color = color;

        if (timer <= 0f)
        {
            Destroy(gameObject);
        }
    }

    private string GetReactionName(
        ElementReactionType reaction)
    {
        return reaction.ToString();
    }

    private Color GetElementColor(
        ElementType elementType)
    {
        switch (elementType)
        {
            case ElementType.Pyro:
                return pyroColor;

            case ElementType.Hydro:
                return hydroColor;

            case ElementType.Cryo:
                return cryoColor;

            case ElementType.Electro:
                return electroColor;

            default:
                return Color.white;
        }
    }
}