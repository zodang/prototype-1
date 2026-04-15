using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public bool IsLinked { get; private set; }
    public float HP = 100;
    
    private SpriteRenderer _spriteRenderer;
    private Coroutine _coroutine;
    
    public Action<Enemy> OnDeath;

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
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
        HP -= damage;
        if (HP <= 0) Die();
    }

    public void IsDetected(bool isDetected)
    {
        _spriteRenderer.color = isDetected ? Color.green : Color.white;
    }
    
    private void Die()
    {
        OnDeath?.Invoke(this);
        Destroy(gameObject);
    }

}