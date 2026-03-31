using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float strength = 3f;

    [SerializeField] private float pullSpeed = 2f;        // 평소 끌림
    [SerializeField] private float absorbSpeed = 10f;     // 죽을 때 빨려가는 속도

    [SerializeField] private Transform _player;
    private bool _isBeingAbsorbed = false;
    private bool _isDead = false;

    private void Start()
    {
        _player = FindAnyObjectByType<PlayerMovement>().transform;
    }

    public void TakeDamage(float amount)
    {
        strength -= amount;

        // 맞고 있는 동안 끌림 활성화
        _isBeingAbsorbed = true;

        if (strength <= 0 && !_isDead)
        {
            _isDead = true;
            Debug.Log("죽음판정");
        }
    }

    private void Update()
    {
        if (_player == null) return;

        Vector3 dir = (_player.position - transform.position).normalized;

        // 1️⃣ 살아있는 동안 → 천천히 끌림
        if (_isBeingAbsorbed && !_isDead)
        {
            transform.position += dir * (pullSpeed * Time.deltaTime);
        }

        // 2️⃣ 죽으면 → 빠르게 빨려 들어감
        if (_isDead)
        {
            transform.position += dir * (absorbSpeed * Time.deltaTime);

            // 플레이어에 거의 도달하면 제거
            float dist = Vector3.Distance(transform.position, _player.position);
            if (dist < 0.2f)
            {
                Destroy(gameObject);
            }
        }
    }

    // 흡수 안될 때 끌림 해제 (선택)
    public void StopAbsorb()
    {
        _isBeingAbsorbed = false;
        strength = 3;
    }
}
