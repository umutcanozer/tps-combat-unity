using System;
using System.Collections;
using UnityEngine;

public class MeleeWeapon : Weapon
{
    [SerializeField] private Collider attackCollider; 
    [SerializeField] private float attackDuration = 0.2f;

    private bool _hasHitTarget = false;

    private void Start()
    {
        attackCollider.enabled = false;
    }

    public override void Attack()
    {
        StartCoroutine(PerformAttack());
    }
    
    private IEnumerator PerformAttack()
    {
        attackCollider.enabled = true;
        Debug.Log("Collider enabled");
        yield return new WaitForSeconds(attackDuration);
        Debug.Log("Collider disabled");
        attackCollider.enabled = false;
        _hasHitTarget = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_hasHitTarget) return;
        if (other.TryGetComponent(out IDamagable target))
        {
            target.TakeDamage(damage);
            _hasHitTarget = true;
        }
    }
}
