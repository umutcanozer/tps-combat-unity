using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private InputActions _inputActions;
    private void OnEnable()
    {
        if (_inputActions == null)
        {
            _inputActions = new InputActions();
        }
        _inputActions.Enable();
        _inputActions.PlayerControls.LockToggle.performed += ctx => MiddleMouseClicked();
        _inputActions.PlayerControls.Attack.performed += ctx => LeftMouseButtonClicked();
        _inputActions.PlayerControls.SwitchTarget.performed += ctx => SetTarget(ctx.ReadValue<Vector2>());
    }
    
    private void Update()
    {
        SetSprint(_inputActions.PlayerControls.Sprint.ReadValue<float>() > 0);
        
        SetKeyboardAxis(_inputActions.PlayerControls.Movement.ReadValue<Vector2>());
        
        SetMouseAxis(_inputActions.PlayerControls.MouseLook.ReadValue<Vector2>());
    }

    private void LeftMouseButtonClicked()
    {
        EventManager.TriggerAttackInput();
    }
    private void MiddleMouseClicked()
    {
        EventManager.TriggerLockInput();
    }

    private void SetTarget(Vector2 direction)
    {
        EventManager.TriggerLockedTargetSwitched(direction);
    }
    
    private void SetSprint(bool state)
    {
        EventManager.TriggerSprintInput(state);
    }

    private void SetKeyboardAxis(Vector2 axis)
    {
        EventManager.TriggerMovementInputs(axis);
    }

    private void SetMouseAxis(Vector2 axis)
    {
        EventManager.TriggerMouseDeltaInput(axis);
    }
    
    private void OnDisable()
    {
        _inputActions.Disable();
    }
}
