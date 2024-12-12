using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    [SerializeField] private Weapon currentWeapon;

    private void OnEnable()
    {
        EventManager.OnAttackInputPerformed += OnAttack;
    }

    private void OnAttack()
    {
        Debug.Log("Attacked");
    }

    private void OnDisable()
    {
        EventManager.OnAttackInputPerformed -= OnAttack;
    }
}
