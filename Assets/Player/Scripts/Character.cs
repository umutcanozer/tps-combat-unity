using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour, IDamagable
{
    [SerializeField] private int health;
    public virtual void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log($"{gameObject.name} has {health} HP left.");

        if (health <= 0)
        {
            Die();
        }
    }
    
    protected virtual void Die()
    {
        Debug.Log($"{gameObject.name} is dead.");
        Destroy(gameObject);
    }
}
