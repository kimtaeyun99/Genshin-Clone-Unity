using UnityEngine;
using UnityEngine.UI;

public class MonsterHealthBar : MonoBehaviour
{
    [Header("HP")]
    [SerializeField] private GameObject healthBarBG;
    [SerializeField] private DelayedHealthBar delayedHealthBar;
    [Header("Element")]
    [SerializeField] private Image hostElementIcon;
    [SerializeField] private Sprite pyroIcon;
    [SerializeField] private Sprite hydroIcon;
    [SerializeField] private Sprite cryoIcon;
    [SerializeField] private Sprite electroIcon;


    [SerializeField] private MonsterController monster;
    private MonsterRunTime runtime;
    private Health health;
    private Camera mainCamera;

    private bool isVisible;

    private void OnEnable()
    {
        Subscribe();
    }

    private void Start()
    {
        mainCamera = Camera.main;
        Subscribe();
        if (runtime == null || health == null)
            return;

        delayedHealthBar.Initialize(runtime.CurrentHP,runtime.MaxHP);

        UpdateElementIcon(health.HostElement);
        SetVisible(false);
    }

    private void Subscribe()
    {
        if (runtime != null || monster == null || monster.Runtime == null ||
            monster.Health == null)
            return;

        runtime = monster.Runtime;
        health = monster.Health;
        runtime.OnHPChanged += UpdateHP;
        health.OnHostElementChanged += UpdateElementIcon;
    }

    private void LateUpdate()
    {
        if (!isVisible)
            return;

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null)
                return;
        }

        transform.rotation = Quaternion.LookRotation(
            transform.position - mainCamera.transform.position);
    }

    public void SetVisible(bool visible)
    {
        Subscribe();

        isVisible = visible;
        healthBarBG.SetActive(visible);

        if (!visible)
        {
            hostElementIcon.gameObject.SetActive(false);
        }
        else if (health != null)
        {
            UpdateElementIcon(health.HostElement);
        }
    }

    private void UpdateHP(float currentHP, float maxHP)
    {
        delayedHealthBar.SetHealth(currentHP, maxHP);
    }
        
    private void UpdateElementIcon(ElementType elementType)
    {
        switch (elementType)
        {
            case ElementType.Pyro:
                hostElementIcon.sprite = pyroIcon;
                break;
            case ElementType.Hydro:
                hostElementIcon.sprite = hydroIcon;
                break;
            case ElementType.Cryo:
                hostElementIcon.sprite = cryoIcon;
                break;
            case ElementType.Electro:
                hostElementIcon.sprite = electroIcon;
                break;
            default:
                hostElementIcon.gameObject.SetActive(false);
                return;
        }

        hostElementIcon.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        if (runtime != null)
            runtime.OnHPChanged -= UpdateHP;
        if (health != null)
            health.OnHostElementChanged -= UpdateElementIcon;

        runtime = null;
        health = null;
    }
}
