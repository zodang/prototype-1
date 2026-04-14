using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public bool IsLinked;
    public float HP = 100;
    
    private Coroutine _coroutine;

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
        HP -= 1;
        if (HP <= 0) Destroy(gameObject);
    }
}