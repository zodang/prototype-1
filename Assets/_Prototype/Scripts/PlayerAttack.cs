using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;

    public int attackRadius = 2;
    public float attackDamage = 1;

    private BoxCollider2D[] detecedEnemy = new BoxCollider2D[6];

    public int maxChainCount = 3;
    private List<Enemy> chain = new List<Enemy>();

    private Coroutine attackCoroutine;

    private void Update()
    {
        if (!inputManager.IsTryingToAttack)
        {
            if (attackCoroutine != null)
            {
                StopCoroutine(attackCoroutine);
                attackCoroutine = null;
            }

            ClearChain();
            return;
        }

        if (attackCoroutine == null)
        {
            attackCoroutine = StartCoroutine(AttackRoutine());
        }
    }

    private IEnumerator AttackRoutine()
    {
        while (inputManager.IsTryingToAttack)
        {
            BuildChain();

            foreach (var enemy in chain)
            {
                if (enemy != null)
                    enemy.TryDamage(attackDamage);
            }

            yield return new WaitForSeconds(1f);
        }

        attackCoroutine = null;
    }

    private void ClearChain()
    {
        foreach (var enemy in chain)
        {
            if (enemy != null)
                enemy.Release();
        }

        chain.Clear();
    }

    private void BuildChain()
    {
        ClearChain();

        Enemy first = FindNearestEnemy(transform.position);
        if (first == null) return;

        chain.Add(first);
        first.Register();

        Enemy current = first;

        while (chain.Count < maxChainCount)
        {
            Enemy next = FindNearestEnemy(current.transform.position);

            if (next == null) break;
            if (chain.Contains(next)) break;

            chain.Add(next);
            next.Register();

            current = next;
        }
    }

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
            if (enemy.IsLinked) continue;

            float dist = Vector2.Distance(center, enemy.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                nearest = enemy;
            }
        }

        return nearest;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0);
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}