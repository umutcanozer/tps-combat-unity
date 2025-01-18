using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    [SerializeField] private Weapon currentWeapon;
    [SerializeField] private float _resetTime = 0.5f;
    
    private AnimationHandler _animatorHandler;
    private int _currentComboIndex = 0;
    private float _timeBetweenAttacks = 0.5f;
    private bool _isAttacking = false;
    public bool IsAttacking => _isAttacking;
    
    private void OnEnable()
    {
        EventManager.OnAttackInputPerformed += OnAttack;
    }

    private void OnAttack()
    {
        if (_isAttacking) return;
        _timeBetweenAttacks = _resetTime;
        Debug.Log(_currentComboIndex);
        _animatorHandler.TriggerAttack(_currentComboIndex);
       
        //currentWeapon.Attack();
        _currentComboIndex++;
        if (_currentComboIndex >= _animatorHandler.ComboHashesLength) ResetCombo();
    }

    private void Start()
    {
        _animatorHandler = AnimationHandler.GetInstance(GetComponent<Animator>());
    }

    private void Update()
    {
        if (_timeBetweenAttacks > 0)
        {
            _timeBetweenAttacks -= Time.deltaTime;
            if (_timeBetweenAttacks <= 0)
            {
                ResetCombo();
            }
        }
    }

    private void ResetCombo()
    {
        _currentComboIndex = 0;
    }

    public void StartAttack()
    {
        _isAttacking = true;
    }
    
    public void EndAttack()
    {
        _isAttacking = false;
        Debug.Log("no attacking");
    }

    private void OnDisable()
    {
        EventManager.OnAttackInputPerformed -= OnAttack;
    }
}
