using System;
using DG.Tweening;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Chase")]
    [SerializeField] private Transform player;
    [SerializeField] private float detectRange = 3f;
    [SerializeField] private float moveSpeed = 0.5f;
    [SerializeField] private float stopDistance = 0.2f;

    [SerializeField] private HPBar hpBar;
    [SerializeField] private DropManager dropManager;
    [SerializeField] private ExplosionEffect hitEffect;
    [SerializeField] private ExplosionEffect explosionEffect;
    public bool IsLinked { get; private set; }
    public float MaxHp = 100;
    public float CurrentHp = 100;
    
    private SpriteRenderer _spriteRenderer;
    
    private Coroutine _coroutine;
    
    public Action<Enemy> OnDeath;

    private Vector3 _originalScale;
    private Color _originalColor;
    private Tween _squashTween;
    private Tween _colorTween;

    private void OnValidate()
    {
        detectRange = Mathf.Max(0f, detectRange);
        moveSpeed = Mathf.Max(0f, moveSpeed);
        stopDistance = Mathf.Max(0f, stopDistance);
    }

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _originalScale = transform.localScale;
        _originalColor = _spriteRenderer.color;
    }
    
    private void Start()
    {
        if (player == null)
        {
            PlayerMovement playerMovement = FindFirstObjectByType<PlayerMovement>();
            if (playerMovement != null)
            {
                player = playerMovement.transform;
            }
        }

        if (dropManager == null)
        {
            dropManager = FindFirstObjectByType<DropManager>();
        }
        
        CurrentHp = MaxHp;
    }

    private void Update()
    {
        ChasePlayerInRange();
    }

    public void Register()
    {
        IsLinked = true;
    }

    public void Release()
    {
        IsLinked = false;
    }

    public void TryDamage(float damage)
    {
        CurrentHp -= damage;
        
        hpBar.SetHP(CurrentHp, MaxHp);
        PlaySquash();
        PlayFlash();
        
        Instantiate(hitEffect, transform.position, Quaternion.identity);
        
        if (CurrentHp <= 0) Die();
    }

    public void IsDetected(bool isDetected)
    {
        _spriteRenderer.color = isDetected ? Color.green : Color.white;
    }

    private void ChasePlayerInRange()
    {
        if (player == null || moveSpeed <= 0f) return;

        Vector2 currentPosition = transform.position;
        Vector2 targetPosition = player.position;
        float distance = Vector2.Distance(currentPosition, targetPosition);

        if (distance > detectRange || distance <= stopDistance) return;

        Vector2 nextPosition = Vector2.MoveTowards(
            currentPosition,
            targetPosition,
            moveSpeed * Time.deltaTime);

        transform.position = new Vector3(nextPosition.x, nextPosition.y, transform.position.z);
    }

    private void PlaySquash()
    {
        _squashTween?.Kill();

        _squashTween = DOTween.Sequence()
            .Append(transform.DOScale(new Vector3(_originalScale.x * 1.5f, _originalScale.y * 0.8f, 1f), 0.06f)
                .SetEase(Ease.OutQuad))
            .Append(transform.DOScale(_originalScale, 0.09f)
                .SetEase(Ease.OutBounce));
    }

    private void PlayFlash()
    {
        _colorTween?.Kill();

        _colorTween = DOTween.Sequence()
            .Append(_spriteRenderer.DOColor(Color.red, 0.05f))
            .Append(_spriteRenderer.DOColor(_originalColor, 0.1f));
    }
    
    private void Die()
    {
        _squashTween?.Kill();
        _colorTween?.Kill();

        Instantiate(explosionEffect, transform.position, Quaternion.identity);
        dropManager.Drop(transform.position);
        
        OnDeath?.Invoke(this);
        Destroy(gameObject);
    }

}
