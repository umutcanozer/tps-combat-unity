using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    [SerializeField] private Weapon currentWeapon;
    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            currentWeapon.Attack();
        }
    }
}
