using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;
    [SerializeField] private float moveSpeed = 3f;
    
    private Vector2 _moveInput;
    
    private void OnEnable()
    {
        inputManager.OnMoveEvent += HandleMove;
    }
    
    private void OnDisable()
    {
        inputManager.OnMoveEvent -= HandleMove;
    }

    private void Update()
    {
        if (!inputManager.IsTryingToMove) return;
        transform.position += (Vector3)(_moveInput * (moveSpeed * Time.deltaTime));
    }

    private void HandleMove(Vector2 value)
    {
        _moveInput = value;
    }
}
