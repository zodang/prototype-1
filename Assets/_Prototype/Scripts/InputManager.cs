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

    private void Awake()
    {
        _inputActions = new PlayerActionsAsset();
        _playerActions = _inputActions.Player;
        _playerActions.Enable();
    }

    private void OnEnable()
    {
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
    }
    
    private void OnMoveStarted(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>();
        IsTryingToMove = true;
        OnMoveStartEvent?.Invoke();
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>();
        OnMoveEvent?.Invoke(MoveInput);
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        MoveInput = Vector2.zero;
        IsTryingToMove = false;
        OnMoveEndEvent?.Invoke();
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
        IsTryingToChainAttack = true;
        OnChainAttackStartedEvent?.Invoke();
    }

    private void OnChainAttackPerformed(InputAction.CallbackContext context)
    {
        OnChainAttackPerformedEvent?.Invoke();
    }
    
    private void OnChainAttackEnded(InputAction.CallbackContext context)
    {
        IsTryingToChainAttack = false;
        OnChainAttackEndedEvent?.Invoke();
    }
}