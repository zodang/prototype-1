using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float moveSpeed = 3f;
    
    public Vector2 direction;
    public float angle;

    private Vector2 _moveInput;
    private Vector2 _lookInput;
    private void OnEnable()
    {
        inputManager.OnMoveEvent += HandleMove;
        inputManager.OnLookEvent += HandleLook;
    }
    
    private void OnDisable()
    {
        inputManager.OnMoveEvent -= HandleMove;
        inputManager.OnLookEvent -= HandleLook;
    }

    private void Start()
    {
        spriteRenderer =  GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        transform.position += (Vector3)(_moveInput * (moveSpeed * Time.deltaTime));
        
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(_lookInput);
        direction = (worldPos - (Vector2)transform.position).normalized;
        angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        
        if (direction.x > 0) spriteRenderer.flipX = false;
        if (direction.x < 0) spriteRenderer.flipX = true;
    }

    private void HandleMove(Vector2 value)
    {
        _moveInput = value;
    }

    private void HandleLook(Vector2 value)
    {
        _lookInput = value;
    }
}
