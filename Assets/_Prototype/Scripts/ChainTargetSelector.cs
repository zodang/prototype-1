using System.Collections.Generic;
using UnityEngine;

/*
범위 내 공격 대상 탐색 및 연결 상태 관리
- 적 탐색
- branch 및 chain 관리
*/

public class ChainTargetSelector
{
    private readonly Collider2D[] _detectedEnemies;
    private readonly ContactFilter2D _contactFilter = ContactFilter2D.noFilter;
    private readonly List<Enemy> _chain = new List<Enemy>();
    private readonly List<Enemy> _previousChain = new List<Enemy>();
    private readonly List<List<Enemy>> _branches = new List<List<Enemy>>();

    public ChainTargetSelector(int detectionBufferSize)
    {
        _detectedEnemies = new Collider2D[Mathf.Max(1, detectionBufferSize)];
    }

    public IReadOnlyList<Enemy> Chain => _chain;
    public IReadOnlyList<List<Enemy>> Branches => _branches;
    public Enemy Current { get; private set; }

    public void UpdateTargets(Vector2 origin, int radius, int maxChainCount, int maxBranchCount)
    {
        _previousChain.Clear();
        _previousChain.AddRange(_chain);

        _chain.Clear();
        _branches.Clear();

        int branchCount = Mathf.Max(maxBranchCount, 1);
        for (int branchIndex = 0; branchIndex < branchCount; branchIndex++)
        {
            _branches.Add(new List<Enemy>());
        }

        int chainCount = Mathf.Max(maxChainCount, 1);
        for (int chainDepth = 0; chainDepth < chainCount; chainDepth++)
        {
            for (int branchIndex = 0; branchIndex < _branches.Count; branchIndex++)
            {
                List<Enemy> branch = _branches[branchIndex];
                Vector2 searchCenter = branch.Count > 0
                    ? (Vector2)branch[branch.Count - 1].transform.position
                    : origin;

                Enemy nearest = FindNearestEnemy(searchCenter, radius);
                if (nearest == null)
                {
                    continue;
                }

                branch.Add(nearest);
                _chain.Add(nearest);
            }
        }

        RemoveEmptyBranches();
        ReleaseEnemiesNoLongerLinked();
        RegisterNewEnemies();

        Current = _chain.Count > 0 ? _chain[0] : null;
    }

    public void RemoveFromChain(Enemy enemy)
    {
        _chain.Remove(enemy);
        for (int i = _branches.Count - 1; i >= 0; i--)
        {
            _branches[i].Remove(enemy);
            if (_branches[i].Count == 0)
            {
                _branches.RemoveAt(i);
            }
        }

        if (Current == enemy)
        {
            Current = null;
        }
    }

    public void Clear()
    {
        foreach (Enemy enemy in _chain)
        {
            if (enemy == null)
            {
                continue;
            }

            enemy.IsDetected(false);
            enemy.Release();
            enemy.OnDeath -= RemoveFromChain;
        }

        _chain.Clear();
        _previousChain.Clear();
        _branches.Clear();
        Current = null;
    }

    private void RemoveEmptyBranches()
    {
        for (int i = _branches.Count - 1; i >= 0; i--)
        {
            if (_branches[i].Count == 0)
            {
                _branches.RemoveAt(i);
            }
        }
    }

    private void ReleaseEnemiesNoLongerLinked()
    {
        foreach (Enemy enemy in _previousChain)
        {
            if (enemy == null || _chain.Contains(enemy))
            {
                continue;
            }

            enemy.IsDetected(false);
            enemy.Release();
            enemy.OnDeath -= RemoveFromChain;
        }
    }

    private void RegisterNewEnemies()
    {
        foreach (Enemy enemy in _chain)
        {
            if (enemy == null || _previousChain.Contains(enemy))
            {
                continue;
            }

            enemy.IsDetected(true);
            enemy.Register();
            enemy.OnDeath += RemoveFromChain;
        }
    }

    private Enemy FindNearestEnemy(Vector2 center, int radius)
    {
        int count = Physics2D.OverlapCircle(center, radius, _contactFilter, _detectedEnemies);

        float minDistance = Mathf.Infinity;
        Enemy nearest = null;

        for (int i = 0; i < count; i++)
        {
            Collider2D collider = _detectedEnemies[i];
            if (collider == null)
            {
                continue;
            }

            Enemy enemy = collider.GetComponent<Enemy>();
            if (enemy == null)
            {
                continue;
            }

            if (enemy.IsLinked && !_previousChain.Contains(enemy))
            {
                continue;
            }

            if (_chain.Contains(enemy))
            {
                continue;
            }

            float distance = Vector2.Distance(center, enemy.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = enemy;
            }
        }

        return nearest;
    }
}
