using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;
    [SerializeField] private CameraEffect cameraEffect;
    private LineRenderer _lineRenderer;

    public int attackRadius = 2;
    public float attackDamage = 1;
    public float attackInterval = 1.0f;

    private BoxCollider2D[] detecedEnemy = new BoxCollider2D[16];

    public int maxChainCount = 3;
    public List<Enemy> chain = new List<Enemy>();

    private Coroutine attackCoroutine;

    private Enemy _current;
    private readonly List<Enemy> _previousChain = new List<Enemy>();

    private void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.positionCount = 0;
    }

    private void Update()
    {
        // 1. 타겟 탐색
        UpdateTarget();

        // 2. 공격 입력 처리
        if (inputManager.IsTryingToAttack)
        {
            HandleAttackStart();
            UpdateLine();
        }
        else
        {
            HandleAttackStop();
        }
    }

    // 타겟 탐색
    private void UpdateTarget()
    {
        _previousChain.Clear();
        _previousChain.AddRange(chain);

        chain.Clear();

        Vector2 searchCenter = transform.position;
        for (int i = 0; i < maxChainCount; i++)
        {
            Enemy nearest = FindNearestEnemy(searchCenter);
            if (nearest == null) break;

            chain.Add(nearest);
            searchCenter = nearest.transform.position;
        }

        foreach (Enemy enemy in _previousChain)
        {
            if (enemy == null || chain.Contains(enemy)) continue;

            enemy.IsDetected(false);
            enemy.Release();
            UnregisterEnemy(enemy);
        }

        foreach (Enemy enemy in chain)
        {
            if (enemy == null || _previousChain.Contains(enemy)) continue;

            enemy.IsDetected(true);
            enemy.Register();
            RegisterEnemy(enemy);
        }

        _current = chain.Count > 0 ? chain[0] : null;
    }
    
    private void RegisterEnemy(Enemy enemy)
    {
        if (enemy == null) return;

        enemy.OnDeath += RemoveFromChain;
    }

    private void UnregisterEnemy(Enemy enemy)
    {
        if (enemy == null) return;

        enemy.OnDeath -= RemoveFromChain;
    }
    
    private void RemoveFromChain(Enemy enemy)
    {
        chain.Remove(enemy);

        if (_current == enemy) _current = null;
    }

    // 공격 시작
    private void HandleAttackStart()
    {
        if (attackCoroutine != null) return;

        attackCoroutine = StartCoroutine(AttackRoutine());
        ActivateLine();
    }


    // 공격 종료
    private void HandleAttackStop()
    {
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }

        _lineRenderer.positionCount = 0;
    }

    // 데미지 루프
    private IEnumerator AttackRoutine()
    {
        while (true)
        {
            if (chain.Count > 0)
            {
                for (int i = chain.Count - 1; i >= 0; i--)
                {
                    if (chain[i] == null)
                    {
                        chain.RemoveAt(i);
                        continue;
                    }

                    chain[i].TryDamage(attackDamage);
                }

                cameraEffect.PlayShake();
            }
                

            yield return new WaitForSeconds(attackInterval);
        }
    }

    // 적 탐색
    private Enemy FindNearestEnemy(Vector2 center)
    {
        int num = Physics2D.OverlapCircleNonAlloc(center, attackRadius, detecedEnemy);

        float minDistance = Mathf.Infinity;
        Enemy nearest = null;

        for (int i = 0; i < num; i++)
        {
            var col = detecedEnemy[i];
            if (col == null) continue;

            var enemy = col.GetComponent<Enemy>();
            if (enemy == null) continue;
            if (enemy.IsLinked && !_previousChain.Contains(enemy)) continue;
            if (chain.Contains(enemy)) continue;

            float dist = Vector2.Distance(center, enemy.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                nearest = enemy;
            }
        }

        return nearest;
    }

    // 라인 처리
    private void ActivateLine()
    {
        _lineRenderer.positionCount = Mathf.Max(chain.Count + 1, 2);
    }

    private void UpdateLine()
    {
        ActivateLine();
        _lineRenderer.SetPosition(0, transform.position);

        if (_current == null)
        {
            _lineRenderer.SetPosition(1, transform.position + Vector3.right * attackRadius);
            return;
        }

        for (int i = 0; i < chain.Count; i++)
        {
            _lineRenderer.SetPosition(i + 1, chain[i].transform.position);
        }
    }
}
