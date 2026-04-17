using System;
using DG.Tweening;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private HPBar hpBar;
    [SerializeField] private DropManager dropManager;
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
    
    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        
        _originalScale = transform.localScale;
        _originalColor = _spriteRenderer.color;

        CurrentHp = MaxHp;
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
        
        if (CurrentHp <= 0) Die();
    }

    public void IsDetected(bool isDetected)
    {
        _spriteRenderer.color = isDetected ? Color.green : Color.white;
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
        
        dropManager.Drop(transform.position);
        OnDeath?.Invoke(this);
        Destroy(gameObject);
    }

}