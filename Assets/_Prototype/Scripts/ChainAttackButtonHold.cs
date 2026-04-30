using UnityEngine;
using UnityEngine.EventSystems;

public class ChainAttackButtonHold : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [SerializeField] private InputManager inputManager;
    [SerializeField] private CanvasGroup uiCanvasGroup;
    

    private bool isHolding;
    [SerializeField] private float defaultAlpha = 1f;
    [SerializeField] private float holdAlpha = 0.1f;

    private void Awake()
    {
        if (inputManager == null)
        {
            inputManager = FindFirstObjectByType<InputManager>();
        }

        if (uiCanvasGroup == null && !TryGetComponent(out uiCanvasGroup))
        {
            uiCanvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        if (uiCanvasGroup != null)
        {
            defaultAlpha = uiCanvasGroup.alpha;
        }
    }

    private void OnDisable()
    {
        EndHold();
        ShowUi();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isHolding || inputManager == null)
        {
            return;
        }

        isHolding = true;
        HideUi();
        inputManager.BeginUiChainAttack();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        EndHold();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        EndHold();
    }

    private void EndHold()
    {
        if (!isHolding || inputManager == null)
        {
            return;
        }

        isHolding = false;
        inputManager.EndUiChainAttack();
        ShowUi();
    }

    private void HideUi()
    {
        if (uiCanvasGroup != null)
        {
            uiCanvasGroup.alpha = holdAlpha;
        }
    }

    private void ShowUi()
    {
        if (uiCanvasGroup != null)
        {
            uiCanvasGroup.alpha = defaultAlpha;
        }
    }
}
