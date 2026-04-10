using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private PlayerActionsAsset _inputActions;
    private PlayerActionsAsset.PlayerActions _playerActions;

    public event Action<Vector2> OnMoveEvent;
    public event Action<Vector2> OnLookEvent;
    public event Action OnJumpEvent;
    public event Action OnAttackEvent;
    
    public Vector2 MoveInput;// { get; private set; }
    public Vector2 LookInput;// { get; private set; }
    public float JumpInput;

    private void Awake()
    {
        _inputActions = new PlayerActionsAsset();
        _playerActions = _inputActions.Player;
        _playerActions.Enable();
    }

    private void OnEnable()
    {
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

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>();
        OnMoveEvent?.Invoke(MoveInput);
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        MoveInput = Vector2.zero;
        OnMoveEvent?.Invoke(MoveInput);
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