using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class ScreenJoystick : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;
    [SerializeField] private RectTransform joystickRoot;
    [SerializeField] private RectTransform handle;
    [SerializeField] private RectTransform excludedArea;
    [SerializeField] private float radius = 120f;

    private CanvasGroup canvasGroup;
    private RectTransform parentRect;
    private int activePointerId = -1;
    private Vector2 originScreenPosition;
    private Vector2 defaultAnchoredPosition;
    private float defaultAlpha = 1f;

    private void Awake()
    {
        if (inputManager == null)
        {
            inputManager = FindFirstObjectByType<InputManager>();
        }

        if (joystickRoot == null)
        {
            joystickRoot = transform as RectTransform;
        }

        if (joystickRoot != null)
        {
            parentRect = joystickRoot.parent as RectTransform;
            defaultAnchoredPosition = joystickRoot.anchoredPosition;
        }

        if (canvasGroup == null && !TryGetComponent(out canvasGroup))
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        if (canvasGroup != null)
        {
            defaultAlpha = canvasGroup.alpha;
        }
    }

    private void OnEnable()
    {
        ResetJoystick();
    }

    private void OnDisable()
    {
        inputManager?.ClearUiMoveInput();
    }

    private void Update()
    {
        if (inputManager == null || joystickRoot == null || parentRect == null)
        {
            return;
        }

        if (activePointerId >= 0)
        {
            UpdateActivePointer();
            return;
        }

        TryStartPointer();
    }

    private void TryStartPointer()
    {
        if (Touchscreen.current != null)
        {
            foreach (TouchControl touch in Touchscreen.current.touches)
            {
                if (!touch.press.wasPressedThisFrame)
                {
                    continue;
                }

                Vector2 screenPosition = touch.position.ReadValue();
                if (IsExcluded(screenPosition))
                {
                    continue;
                }

                BeginPointer(touch.touchId.ReadValue(), screenPosition);
                return;
            }
        }

        if (Mouse.current == null || !Mouse.current.leftButton.wasPressedThisFrame)
        {
            return;
        }

        Vector2 mousePosition = Mouse.current.position.ReadValue();
        if (!IsExcluded(mousePosition))
        {
            BeginPointer(0, mousePosition);
        }
    }

    private void UpdateActivePointer()
    {
        if (Touchscreen.current != null)
        {
            foreach (TouchControl touch in Touchscreen.current.touches)
            {
                if (touch.touchId.ReadValue() != activePointerId)
                {
                    continue;
                }

                if (!touch.press.isPressed)
                {
                    EndPointer();
                    return;
                }

                UpdateDrag(touch.position.ReadValue());
                return;
            }
        }

        if (activePointerId == 0 && Mouse.current != null)
        {
            if (!Mouse.current.leftButton.isPressed)
            {
                EndPointer();
                return;
            }

            UpdateDrag(Mouse.current.position.ReadValue());
        }
    }

    private void BeginPointer(int pointerId, Vector2 screenPosition)
    {
        activePointerId = pointerId;
        originScreenPosition = screenPosition;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, screenPosition, null, out Vector2 localPosition))
        {
            joystickRoot.anchoredPosition = localPosition;
        }

        ShowJoystick();
        UpdateDrag(screenPosition);
    }

    private void UpdateDrag(Vector2 screenPosition)
    {
        Vector2 delta = Vector2.ClampMagnitude(screenPosition - originScreenPosition, radius);
        Vector2 moveInput = radius <= 0f ? Vector2.zero : delta / radius;

        if (handle != null)
        {
            handle.anchoredPosition = delta;
        }

        inputManager.SetUiMoveInput(moveInput);
    }

    private void EndPointer()
    {
        activePointerId = -1;
        inputManager.ClearUiMoveInput();
        ResetJoystick();
    }

    private void ResetJoystick()
    {
        if (joystickRoot != null)
        {
            joystickRoot.anchoredPosition = defaultAnchoredPosition;
        }

        if (handle != null)
        {
            handle.anchoredPosition = Vector2.zero;
        }

        ShowJoystick();
    }

    private void ShowJoystick()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = defaultAlpha;
        }
    }

    private bool IsExcluded(Vector2 screenPosition)
    {
        return excludedArea != null && RectTransformUtility.RectangleContainsScreenPoint(excludedArea, screenPosition);
    }
}
