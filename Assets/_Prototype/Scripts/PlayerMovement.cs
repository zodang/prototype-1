using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private PlayerAbsorption absorption;
    [SerializeField] private float moveSpeed = 5f;

    private Vector2 _moveInput;
    private Vector2 _lookDirection = Vector2.right;

    public void OnMove(InputValue value)
    {
        _moveInput = value.Get<Vector2>();
    }

    private void Update()
    {
        Move();
        UpdateLookDirectionByMouse();
        RotateToLookDirection();
    }

    private void Move()
    {
        transform.position += (Vector3)(_moveInput * moveSpeed * Time.deltaTime);
    }

    private void UpdateLookDirectionByMouse()
    {
        Vector3 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);

        Vector2 dir = mouseWorldPos - transform.position;

        // z값 제거 (2D니까)
        dir.Normalize();

        if (dir.sqrMagnitude > 0.001f)
        {
            _lookDirection = dir;
            absorption.SetLookDirection(_lookDirection);
        }
    }

    private void RotateToLookDirection()
    {
        float angle = Mathf.Atan2(_lookDirection.y, _lookDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        Vector3 start = transform.position;
        Vector3 direction = new Vector3(_lookDirection.x, _lookDirection.y, 0f);

        Gizmos.DrawLine(start, start + direction * 2f);
    }
}