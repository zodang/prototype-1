using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerHp : MonoBehaviour
{
    [FormerlySerializedAs("duration")]
    [SerializeField] private float maxHp = 30f;

    private readonly HashSet<Enemy> damagedEnemies = new HashSet<Enemy>();

    private float currentHp;
    private bool hasEnded;

    public event Action OnHpEnded;
    public event Action<float, float> OnHpChanged;

    public float MaxHp => maxHp;
    public float CurrentHp => currentHp;
    public float NormalizedHp => maxHp <= 0f ? 0f : Mathf.Clamp01(currentHp / maxHp);
    public bool IsAlive => !hasEnded;

    private void OnValidate()
    {
        maxHp = Mathf.Max(0f, maxHp);
    }

    private void Awake()
    {
        ResetHp();
    }

    private void OnDisable()
    {
        damagedEnemies.Clear();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryTakeDamageFrom(collision.collider);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        TryTakeDamageFrom(collision.collider);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryTakeDamageFrom(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TryTakeDamageFrom(other);
    }

    public void ResetHp()
    {
        currentHp = Mathf.Max(0f, maxHp);
        hasEnded = currentHp <= 0f;
        damagedEnemies.Clear();
        OnHpChanged?.Invoke(currentHp, maxHp);

        if (hasEnded)
        {
            OnHpEnded?.Invoke();
        }
    }

    public void ReduceHp(float amount)
    {
        if (hasEnded || amount <= 0f)
        {
            return;
        }

        currentHp = Mathf.Max(0f, currentHp - amount);
        OnHpChanged?.Invoke(currentHp, maxHp);

        if (currentHp <= 0f)
        {
            EndHp();
        }
    }

    private void TryTakeDamageFrom(Collider2D other)
    {
        Enemy enemy = other.GetComponentInParent<Enemy>();
        if (hasEnded || enemy == null || damagedEnemies.Contains(enemy))
        {
            return;
        }

        damagedEnemies.Add(enemy);
        ReduceHp(enemy.ContactDamage);
        enemy.DestroyByPlayerContact();
    }

    private void EndHp()
    {
        if (hasEnded)
        {
            return;
        }

        currentHp = 0f;
        hasEnded = true;

        OnHpEnded?.Invoke();
    }
}
