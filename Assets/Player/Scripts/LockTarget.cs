using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

public class LockTarget : MonoBehaviour
{
    /*
     * world position to screen position
     * if the coordinates greater than half of the width, it's on the right side of the screen
     * cam.WorldToScreen
     */
    [SerializeField] private float sphereRadius = 20f;
    [SerializeField] private LayerMask targetableLayer;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private Camera _cam;
    
    private AnimationHandler _animatorHandler;
    
    private bool _isLocked;
    public bool IsLocked => _isLocked;
    private List<GameObject> _targetableObjects;
    private List<GameObject> _targetableObjectsSide;
    
    private GameObject _targetObject;
    public GameObject TargetObject => _targetObject;
    
    
    private void OnEnable()
    {
        EventManager.OnLockInputPerformed += OnLockStateChange;
        EventManager.OnLockedTargetSwitched += OnLockedTargetSwitch;
    }

    private void OnLockStateChange()
    {
        if (_isLocked)
        {
            UnlockTarget();
            return;
        }

        LockATarget();

        
    }

    private void OnLockedTargetSwitch(Vector2 direction)
    {
        if (_targetableObjects.Count <= 1) return;
        Vector3 targetScreenPos = _cam.WorldToScreenPoint(_targetObject.transform.position);

        
        if (direction.x > 0)
        {
            SwitchToRightTarget(targetScreenPos);
        }
        else if (direction.x < 0)
        {
            SwitchToLeftTarget(targetScreenPos);
        }
        
    }
   
    private void OnDisable()
    {
        EventManager.OnLockInputPerformed -= OnLockStateChange;
        EventManager.OnLockedTargetSwitched -= OnLockedTargetSwitch;
    }
    
    void Start()
    {
        _animatorHandler = AnimationHandler.GetInstance(GetComponent<Animator>());
        _targetableObjectsSide = new List<GameObject>();
        _targetableObjects = new List<GameObject>();
    }

    private void Update()
    {
        //Debug.Log(_isLocked);
        //Debug.Log(_targetObject);
        Debug.Log(_targetableObjects.Count.ToString() + "targets");
        if(_targetableObjectsSide.Count > 0 && _targetableObjectsSide != null) Debug.Log(_targetableObjectsSide.Count.ToString());
    }
    
    private void UnlockTarget()
    {
        _targetableObjects.Clear();
        _isLocked = false;
        _targetObject = null;
        AdjustAnimationAndLockState();
    }

    private void LockATarget()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, sphereRadius, targetableLayer);
        foreach (var hitCollider in hitColliders)
        {
            ITargetable targetable = hitCollider.GetComponent<ITargetable>();
            if (targetable == null) continue;
            RayToTargets(targetable);
        }
        
        _isLocked = _targetableObjects.Count > 0;
        if(_isLocked) _targetObject = FindClosestTarget(_targetableObjects, this.transform);
        AdjustAnimationAndLockState();
    }

    private void RayToTargets(ITargetable targetable)
    {
        Vector3 directionToTarget = (targetable.transform.position - transform.position).normalized;
                
        if (Physics.Raycast(transform.position, directionToTarget, out RaycastHit hit, sphereRadius,
                targetableLayer | obstacleLayer))
        {
            Debug.DrawLine(transform.position, hit.point, Color.red, 2.0f);
            if (hit.collider.gameObject != targetable.gameObject) return;
            _targetableObjects.Add(targetable.gameObject);
        }
    }

    private GameObject FindClosestTarget(List<GameObject> targets, Transform t)
    {
        GameObject closestTarget = targets[0];
        float closestDistance = Vector3.Distance(t.position, closestTarget.transform.position);
    
        for (int i = 1; i < targets.Count; i++) 
        {
            float distance = Vector3.Distance(t.position, targets[i].transform.position);
        
            if (distance < closestDistance)
            {
                closestTarget = targets[i];
                closestDistance = distance;
            }
        }

        return closestTarget;
    }
    
    private void SwitchToRightTarget(Vector3 targetScreenPos)
    {
        _targetableObjectsSide.Clear();
        foreach (var target in _targetableObjects)
        {
            if(target == _targetObject) continue;
            Vector3 screenPos = _cam.WorldToScreenPoint (target.transform.position);
            if(screenPos.x > targetScreenPos.x) _targetableObjectsSide.Add(target);
        }
        
        CheckSideList();
    }
    
    private void SwitchToLeftTarget(Vector3 targetScreenPos)
    {
        _targetableObjectsSide.Clear();
        foreach (var target in _targetableObjects)
        {
            if(target == _targetObject) continue;
            Vector3 screenPos = _cam.WorldToScreenPoint (target.transform.position);
            if(screenPos.x < targetScreenPos.x) _targetableObjectsSide.Add(target);
        }
        
        CheckSideList();
    }
    
    private void CheckSideList()
    {
        if (_targetableObjectsSide.Count > 0)
        {
            _targetObject = FindClosestTarget(_targetableObjectsSide, transform);
            AdjustAnimationAndLockState();
        }
    }
    
    private void AdjustAnimationAndLockState()
    {
        _animatorHandler.SetLockStatus(_isLocked);
        EventManager.TriggerLockStateChanged(_isLocked, _targetObject);
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, sphereRadius);
    }
}
