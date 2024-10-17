using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    
    [SerializeField] private float distanceLocked = 5f;
    [SerializeField] private float distanceUnlocked = 10f;
    [SerializeField] private float heightUnlocked = 5f;
    [SerializeField] private float heightLocked = 2f;
    [SerializeField] private float horizontalAdjustValue = 2f;
    [SerializeField] private float smoothSpeed = 1f;

    [SerializeField] private float mouseSensitivity = 3f;
    
    private GameObject _target;
    
    private float _pitch;
    private float _yaw;
    private Quaternion _originalRotation;
    
    private bool _isLocked;
    private void OnEnable()
    {
        EventManager.OnLockStateChanged += OnLockBoolChange;
        EventManager.OnMouseLookInputPerformed += OnMouseLookChange;
    }
    
    private void OnLockBoolChange(bool isLocked, GameObject target)
    {
        _isLocked = isLocked;
        _target = target;
    }

    private void OnMouseLookChange(Vector2 axis)
    {
        _yaw += axis.x * mouseSensitivity * Time.deltaTime; 
        _pitch -= axis.y * mouseSensitivity * Time.deltaTime;  
        _pitch = Mathf.Clamp(_pitch, 10f, 45f);  
    }

    private void OnDisable()
    {
        EventManager.OnLockStateChanged -= OnLockBoolChange;
        EventManager.OnMouseLookInputPerformed -= OnMouseLookChange;
    }

    private void Start()
    {
        _originalRotation = transform.rotation;
    }

    private void LateUpdate()
    {
        UpdateCameraPosition(_isLocked);
    }
    
    private void UpdateCameraPosition(bool isLocked)
    {
        if (isLocked)
        {
            Vector3 targetPos = playerTransform.position - playerTransform.forward * distanceLocked + playerTransform.right * horizontalAdjustValue;
            Vector3 adjustedTargetPos = new Vector3(targetPos.x , targetPos.y + heightLocked, targetPos.z);

            transform.position = Vector3.Lerp(transform.position, adjustedTargetPos, smoothSpeed);
            transform.LookAt(_target.transform);
        }
        else
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, _originalRotation, smoothSpeed * Time.deltaTime);
            Vector3 targetPos = playerTransform.position;
            Vector3 adjustedTargetPos = new Vector3(targetPos.x - distanceUnlocked, targetPos.y + heightUnlocked, targetPos.z);
            
            transform.position = Vector3.Lerp(transform.position, adjustedTargetPos, smoothSpeed);
            HandleMouseMovement();
        }
    }

    private void HandleMouseMovement()
    {
        Vector3 direction = new Vector3(0, 0, -distanceUnlocked);  
        Quaternion rotation = Quaternion.Euler(_pitch, _yaw, 0);  
        Vector3 position = playerTransform.position + rotation * direction;  

        transform.position = Vector3.Lerp(transform.position, position, smoothSpeed);  
        transform.LookAt(playerTransform); 
    }
}
