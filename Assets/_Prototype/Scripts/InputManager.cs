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
    public event Action OnAttackEvent;
    
    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }
    public float JumpInput;

    public bool IsMoving { get; private set; }

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
    }
    
    private void OnDisable()
    {
        _playerActions.Move.performed -= OnMovePerformed;
        _playerActions.Move.canceled -= OnMoveCanceled;
        _playerActions.Look.performed -= OnLookPerformed;
        _playerActions.Jump.performed -= OnJumpPerformed;
    }
    
    private void OnMoveStarted(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>();
        IsMoving = true;
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
        IsMoving = false;
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
    
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Attack");
        }
        else if (context.canceled)
        {
            Debug.Log("CANCEL Attack");
        }
    }
}