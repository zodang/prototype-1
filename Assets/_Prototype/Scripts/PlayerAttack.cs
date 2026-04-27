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
    public int maxChainBranchCount = 1;
    public List<Enemy> chain = new List<Enemy>();

    private Coroutine attackCoroutine;

    private Enemy _current;
    private readonly List<Enemy> _previousChain = new List<Enemy>();
    private readonly List<List<Enemy>> _chainBranches = new List<List<Enemy>>();
    private readonly List<LineRenderer> _branchLineRenderers = new List<LineRenderer>();

    private void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _branchLineRenderers.Add(_lineRenderer);
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
        _chainBranches.Clear();

        int branchCount = Mathf.Max(maxChainBranchCount, 1);
        for (int branchIndex = 0; branchIndex < branchCount; branchIndex++)
        {
            _chainBranches.Add(new List<Enemy>());
        }

        for (int chainDepth = 0; chainDepth < maxChainCount; chainDepth++)
        {
            for (int branchIndex = 0; branchIndex < _chainBranches.Count; branchIndex++)
            {
                List<Enemy> branch = _chainBranches[branchIndex];
                Vector2 searchCenter = branch.Count > 0
                    ? (Vector2)branch[branch.Count - 1].transform.position
                    : (Vector2)transform.position;

                Enemy nearest = FindNearestEnemy(searchCenter);
                if (nearest == null) continue;

                branch.Add(nearest);
                chain.Add(nearest);
            }
        }

        for (int i = _chainBranches.Count - 1; i >= 0; i--)
        {
            if (_chainBranches[i].Count == 0)
            {
                _chainBranches.RemoveAt(i);
            }
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
        for (int i = _chainBranches.Count - 1; i >= 0; i--)
        {
            _chainBranches[i].Remove(enemy);
            if (_chainBranches[i].Count == 0)
            {
                _chainBranches.RemoveAt(i);
            }
        }

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

        ClearLines();
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
        if (_chainBranches.Count == 0)
        {
            _lineRenderer.positionCount = 2;

            for (int i = 1; i < _branchLineRenderers.Count; i++)
            {
                _branchLineRenderers[i].positionCount = 0;
            }

            return;
        }

        for (int i = 0; i < _chainBranches.Count; i++)
        {
            LineRenderer lineRenderer = GetLineRenderer(i);
            lineRenderer.positionCount = Mathf.Max(_chainBranches[i].Count + 1, 2);
        }

        for (int i = _chainBranches.Count; i < _branchLineRenderers.Count; i++)
        {
            _branchLineRenderers[i].positionCount = 0;
        }
    }

    private void UpdateLine()
    {
        ActivateLine();

        if (_chainBranches.Count == 0)
        {
            _lineRenderer.SetPosition(0, transform.position);
            _lineRenderer.SetPosition(1, transform.position + Vector3.right * attackRadius);
            return;
        }

        for (int branchIndex = 0; branchIndex < _chainBranches.Count; branchIndex++)
        {
            LineRenderer lineRenderer = GetLineRenderer(branchIndex);
            List<Enemy> branch = _chainBranches[branchIndex];

            lineRenderer.SetPosition(0, transform.position);
            for (int i = 0; i < branch.Count; i++)
            {
                lineRenderer.SetPosition(i + 1, branch[i].transform.position);
            }
        }
    }

    private LineRenderer GetLineRenderer(int index)
    {
        while (_branchLineRenderers.Count <= index)
        {
            LineRenderer lineRenderer = CreateBranchLineRenderer(_branchLineRenderers.Count);
            _branchLineRenderers.Add(lineRenderer);
        }

        return _branchLineRenderers[index];
    }

    private LineRenderer CreateBranchLineRenderer(int index)
    {
        GameObject lineObject = new GameObject($"ChainBranchLine_{index}");
        lineObject.transform.SetParent(transform);

        LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();
        lineRenderer.useWorldSpace = _lineRenderer.useWorldSpace;
        lineRenderer.material = _lineRenderer.material;
        lineRenderer.startWidth = _lineRenderer.startWidth;
        lineRenderer.endWidth = _lineRenderer.endWidth;
        lineRenderer.startColor = _lineRenderer.startColor;
        lineRenderer.endColor = _lineRenderer.endColor;
        lineRenderer.sortingLayerID = _lineRenderer.sortingLayerID;
        lineRenderer.sortingOrder = _lineRenderer.sortingOrder;
        lineRenderer.textureMode = _lineRenderer.textureMode;
        lineRenderer.alignment = _lineRenderer.alignment;
        lineRenderer.numCapVertices = _lineRenderer.numCapVertices;
        lineRenderer.numCornerVertices = _lineRenderer.numCornerVertices;
        lineRenderer.positionCount = 0;

        return lineRenderer;
    }

    private void ClearLines()
    {
        for (int i = 0; i < _branchLineRenderers.Count; i++)
        {
            _branchLineRenderers[i].positionCount = 0;
        }
    }
}
