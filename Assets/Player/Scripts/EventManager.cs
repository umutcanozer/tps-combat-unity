using UnityEngine.Events;
using UnityEngine;

public static class EventManager
{
    public static event UnityAction<bool, GameObject> OnLockStateChanged;
    public static event UnityAction<Vector2> OnLockedTargetSwitched; 
    public static event UnityAction OnLockInputPerformed;
    public static event UnityAction<bool> OnSprintInputPerformed;
    public static event UnityAction<Vector2> OnMovementInputsPerformed;
    public static event UnityAction<Vector2> OnMouseLookInputPerformed;
    public static event UnityAction OnAttackInputPerformed;
    public static void TriggerLockStateChanged(bool isLocked, GameObject target)
    {
        OnLockStateChanged?.Invoke(isLocked, target);
    }

    public static void TriggerLockedTargetSwitched(Vector2 direction)
    {
        OnLockedTargetSwitched?.Invoke(direction);
    }
    
    public static void TriggerLockInput()
    {
        OnLockInputPerformed?.Invoke();
    }

    public static void TriggerAttackInput()
    {
        OnAttackInputPerformed?.Invoke();
    }

    public static void TriggerSprintInput(bool isSprinting)
    {
        OnSprintInputPerformed?.Invoke(isSprinting);
    }

    public static void TriggerMovementInputs(Vector2 axis)
    {
        OnMovementInputsPerformed?.Invoke(axis);
    }

    public static void TriggerMouseDeltaInput(Vector2 axis)
    {
        OnMouseLookInputPerformed?.Invoke(axis);
    }
}
