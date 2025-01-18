using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHandler
{
    private static AnimationHandler _instance;
    private Animator _anim;

    private float _currentSpeed;
    private float _currentVertical, _currentHorizontal;
    private float _verticalVelocity, _horizontalVelocity;
    private float _animVertical, _animHorizontal;

    private readonly int _verticalHash = Animator.StringToHash("vertical");
    private readonly int _horizontalHash = Animator.StringToHash("horizontal");
    private readonly int _isLockedHash = Animator.StringToHash("isLocked");
    private readonly int _speedHash = Animator.StringToHash("speed");
    private static readonly int[] _comboHashes = {
        Animator.StringToHash("attack0"),
        Animator.StringToHash("attack1"),
        Animator.StringToHash("attack2"),
        Animator.StringToHash("attack3")
    };

    public int ComboHashesLength
    {
        get => _comboHashes.Length;
    }


    private bool _isAttacking;

    private AnimationHandler(Animator animator)
    {
        _anim = animator;
    }
    
    public static AnimationHandler GetInstance(Animator animator)
    {
        if (_instance == null)
        {
            _instance = new AnimationHandler(animator);
        }
        return _instance;
    }

    public void TriggerAttack(int comboIndex)
    {
        _anim.SetTrigger(_comboHashes[comboIndex]);
    }
    
    public void SetLockStatus(bool isLocked)
    {
        _anim.SetBool(_isLockedHash, isLocked);
    }
    
    public void SetLockedSprintParameters(float horizontal, float vertical)
    {
        _animVertical = vertical;
        _animHorizontal = horizontal;
    }

    public void UpdateMovementParameters(float speed)
    {
        float refVelocity = 0f;
        _currentSpeed = Mathf.SmoothDamp(_currentSpeed, speed, ref refVelocity, 0.1f);
        _anim.SetFloat(_speedHash, _currentSpeed);
    }

    public void UpdateLockedMovementParameters(float verticalInput, float horizontalInput)
    {
        _currentVertical = Mathf.SmoothDamp(_currentVertical, verticalInput * _animVertical, ref _verticalVelocity, 0.1f);
        _currentHorizontal = Mathf.SmoothDamp(_currentHorizontal, horizontalInput * _animHorizontal, ref _horizontalVelocity, 0.1f);

        _anim.SetFloat(_verticalHash, _currentVertical);
        _anim.SetFloat(_horizontalHash, _currentHorizontal);
    }

    /*public bool AttackAdjustment()
    {
        if (IsAttackAnimationFinished()) _isAttacking = true;
        else _isAttacking = false;

        return _isAttacking;
    }

    private bool IsAttackAnimationFinished()
    {
        AnimatorStateInfo currentState = _anim.GetCurrentAnimatorStateInfo(0);
        return currentState.IsTag("Attack");
    }*/
}



