using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    private Vector2 _moveInput;

    public void OnMove(InputValue value)
    {
        _moveInput = value.Get<Vector2>();
    }

    private void Update()
    {
        transform.position += (Vector3)(_moveInput * (moveSpeed * Time.deltaTime));
    }
}