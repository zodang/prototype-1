using System;
using Unity.VisualScripting;
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
    public event Action OnJumpEvent;
    public event Action OnAttackStartedEvent;
    public event Action OnAttackPerformedEvent;
    public event Action OnAttackEndedEvent;
    
    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }
    public float JumpInput;

    public bool IsTryingToMove { get; private set; }
    public bool IsTryingToAttack { get; private set; }

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
        _playerActions.Jump.performed += OnJumpPerformed;

        _playerActions.Attack.started += OnAttackStarted;
        _playerActions.Attack.performed += OnAttackPerformed;
        _playerActions.Attack.canceled += OnAttackEnded;
    }
    
    private void OnDisable()
    {
        _playerActions.Move.started -= OnMoveStarted;
        _playerActions.Move.performed -= OnMovePerformed;
        _playerActions.Move.canceled -= OnMoveCanceled;
        
        _playerActions.Look.performed -= OnLookPerformed;
        _playerActions.Jump.performed -= OnJumpPerformed;
        
        _playerActions.Attack.started -= OnAttackStarted;
        _playerActions.Attack.canceled -= OnAttackEnded;
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

    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        JumpInput = context.ReadValue<float>();
        OnJumpEvent?.Invoke();
    }
    
    private void OnAttackStarted(InputAction.CallbackContext context)
    {
        IsTryingToAttack = true;
        OnAttackStartedEvent?.Invoke();
    }

    private void OnAttackPerformed(InputAction.CallbackContext context)
    {
        OnAttackPerformedEvent?.Invoke();
    }
    
    private void OnAttackEnded(InputAction.CallbackContext context)
    {
        IsTryingToAttack = false;
        OnAttackEndedEvent?.Invoke();
    }
}