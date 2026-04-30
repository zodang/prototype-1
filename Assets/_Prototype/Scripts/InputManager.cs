using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private PlayerActionsAsset _inputActions;
    private PlayerActionsAsset.PlayerActions _playerActions;

    public event Action OnMoveStartEvent;
    public event Action<Vector2> OnMoveEvent;
    public event Action OnMoveEndEvent;
    
    public event Action<Vector2> OnLookEvent;

    public event Action OnBasicAttackStartedEvent;
    public event Action OnBasicAttackPerformedEvent;
    public event Action OnBasicAttackEndedEvent;

    public event Action OnChainAttackStartedEvent;
    public event Action OnChainAttackPerformedEvent;
    public event Action OnChainAttackEndedEvent;
    
    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }

    public bool IsTryingToMove { get; private set; }
    public bool IsTryingToBasicAttack { get; private set; }
    public bool IsTryingToChainAttack { get; private set; }

    private Vector2 _keyboardMoveInput;
    private Vector2 _uiMoveInput;
    private bool _isKeyboardChainAttackPressed;
    private bool _isUiChainAttackPressed;

    private void Awake()
    {
        _inputActions = new PlayerActionsAsset();
        _playerActions = _inputActions.Player;
    }

    private void OnEnable()
    {
        _playerActions.Enable();

        _playerActions.Move.started += OnMoveStarted;
        _playerActions.Move.performed += OnMovePerformed;
        _playerActions.Move.canceled += OnMoveCanceled;
        
        _playerActions.Look.performed += OnLookPerformed;

        _playerActions.BasicAttack.started += OnBasicAttackStarted;
        _playerActions.BasicAttack.performed += OnBasicAttackPerformed;
        _playerActions.BasicAttack.canceled += OnBasicAttackEnded;

        _playerActions.ChainAttack.started += OnChainAttackStarted;
        _playerActions.ChainAttack.performed += OnChainAttackPerformed;
        _playerActions.ChainAttack.canceled += OnChainAttackEnded;
        
    }
    
    private void OnDisable()
    {
        _playerActions.Move.started -= OnMoveStarted;
        _playerActions.Move.performed -= OnMovePerformed;
        _playerActions.Move.canceled -= OnMoveCanceled;
        
        _playerActions.Look.performed -= OnLookPerformed;

        _playerActions.BasicAttack.started -= OnBasicAttackStarted;
        _playerActions.BasicAttack.performed -= OnBasicAttackPerformed;
        _playerActions.BasicAttack.canceled -= OnBasicAttackEnded;
        
        _playerActions.ChainAttack.started -= OnChainAttackStarted;
        _playerActions.ChainAttack.performed -= OnChainAttackPerformed;
        _playerActions.ChainAttack.canceled -= OnChainAttackEnded;

        _playerActions.Disable();
    }

    private void OnDestroy()
    {
        _inputActions?.Dispose();
    }
    
    private void OnMoveStarted(InputAction.CallbackContext context)
    {
        _keyboardMoveInput = context.ReadValue<Vector2>();
        UpdateMoveState();
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        _keyboardMoveInput = context.ReadValue<Vector2>();
        UpdateMoveState();
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        _keyboardMoveInput = Vector2.zero;
        UpdateMoveState();
    }

    private void OnLookPerformed(InputAction.CallbackContext context)
    {
        LookInput = context.ReadValue<Vector2>();
        OnLookEvent?.Invoke(LookInput);
    }

    private void OnBasicAttackStarted(InputAction.CallbackContext context)
    {
        IsTryingToBasicAttack = true;
        OnBasicAttackStartedEvent?.Invoke();
    }

    private void OnBasicAttackPerformed(InputAction.CallbackContext context)
    {
        OnBasicAttackPerformedEvent?.Invoke();
    }
    
    private void OnBasicAttackEnded(InputAction.CallbackContext context)
    {
        IsTryingToBasicAttack = false;
        OnBasicAttackEndedEvent?.Invoke();
    }
    
    private void OnChainAttackStarted(InputAction.CallbackContext context)
    {
        _isKeyboardChainAttackPressed = true;
        UpdateChainAttackState();
    }

    private void OnChainAttackPerformed(InputAction.CallbackContext context)
    {
        OnChainAttackPerformedEvent?.Invoke();
    }
    
    private void OnChainAttackEnded(InputAction.CallbackContext context)
    {
        _isKeyboardChainAttackPressed = false;
        UpdateChainAttackState();
    }

    public void BeginUiChainAttack()
    {
        _isUiChainAttackPressed = true;
        UpdateChainAttackState();
        OnChainAttackPerformedEvent?.Invoke();
    }

    public void EndUiChainAttack()
    {
        _isUiChainAttackPressed = false;
        UpdateChainAttackState();
    }

    public void SetUiMoveInput(Vector2 moveInput)
    {
        _uiMoveInput = Vector2.ClampMagnitude(moveInput, 1f);
        UpdateMoveState();
    }

    public void ClearUiMoveInput()
    {
        _uiMoveInput = Vector2.zero;
        UpdateMoveState();
    }

    private void UpdateMoveState()
    {
        Vector2 previousMoveInput = MoveInput;
        bool wasTryingToMove = IsTryingToMove;

        MoveInput = Vector2.ClampMagnitude(_keyboardMoveInput + _uiMoveInput, 1f);
        IsTryingToMove = MoveInput.sqrMagnitude > 0.0001f;

        if (!wasTryingToMove && IsTryingToMove)
        {
            OnMoveStartEvent?.Invoke();
        }

        if (IsTryingToMove && previousMoveInput != MoveInput)
        {
            OnMoveEvent?.Invoke(MoveInput);
        }

        if (wasTryingToMove && !IsTryingToMove)
        {
            OnMoveEndEvent?.Invoke();
        }
    }

    private void UpdateChainAttackState()
    {
        bool isTryingToChainAttack = _isKeyboardChainAttackPressed || _isUiChainAttackPressed;
        if (IsTryingToChainAttack == isTryingToChainAttack)
        {
            return;
        }

        IsTryingToChainAttack = isTryingToChainAttack;

        if (IsTryingToChainAttack)
        {
            OnChainAttackStartedEvent?.Invoke();
        }
        else
        {
            OnChainAttackEndedEvent?.Invoke();
        }
    }
}
