using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;
    
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    
    private Vector2 _lookInput;
    
    private Vector2 _direction;
    private float _angle;
    
    private static readonly int IsRunning = Animator.StringToHash("b_running");
    
    private void OnEnable()
    {
        inputManager.OnMoveStartEvent += HandleMoveStarted;
        inputManager.OnMoveEndEvent += HandleMoveEnded;
        
        inputManager.OnLookEvent += HandleLook;
    }
    
    private void OnDisable()
    {
        inputManager.OnMoveStartEvent -= HandleMoveStarted;
        inputManager.OnMoveEndEvent -= HandleMoveEnded;
        
        inputManager.OnLookEvent -= HandleLook;
    }
    

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer =  GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(_lookInput);
        _direction = (worldPos - (Vector2)transform.position).normalized;
        _angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
        
        if (_direction.x > 0) _spriteRenderer.flipX = false;
        if (_direction.x < 0) _spriteRenderer.flipX = true;
    }

    private void HandleMoveStarted()
    {
        _animator.SetBool(IsRunning, true);
    }
    
    private void HandleMoveEnded()
    {
        _animator.SetBool(IsRunning, false);
    }

    private void HandleLook(Vector2 value)
    {
        _lookInput = value;
    }
    
}
