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
    [SerializeField] private float sphereRadius = 20f;
    [SerializeField] private LayerMask targetableLayer;
    [SerializeField] private LayerMask obstacleLayer;
    
    private AnimationHandler _animatorHandler;
    
    private bool _isLocked;
    public bool IsLocked => _isLocked;
    private List<GameObject> _targetableObjects;
    
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
            _targetableObjects.Clear();
            _isLocked = false;
            _targetObject = null;
            AdjustAnimationAndLockState();
            return;
        }

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, sphereRadius, targetableLayer);
        _targetableObjects = new List<GameObject>();

        foreach (var hitCollider in hitColliders)
        {
            ITargetable targetable = hitCollider.GetComponent<ITargetable>();
            if (targetable == null) continue;
            RayToTargets(targetable);
        }
        
        _isLocked = _targetableObjects.Count > 0;
        _targetObject = FindClosestTarget(_targetableObjects, this.transform);
        AdjustAnimationAndLockState();
    }

    private void OnLockedTargetSwitch(Vector2 direction)
    {
        if (_targetableObjects.Count <= 1) return;
        
        if (direction.x > 0)
        {
            Debug.Log("sag");
            SwitchToRightTarget();

        }
        else if (direction.x < 0)
        {
            Debug.Log("sol");
            SwitchToLeftTarget();
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
    }

    private void Update()
    {
        Debug.Log(_isLocked);
        Debug.Log(_targetObject);
        Debug.Log(_targetableObjects.Count.ToString());
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
        if (targets.Count == 0) return null;

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

    private void SwitchToRightTarget()
    {
        //switch right
    }
    
    private void SwitchToLeftTarget()
    {
        //switch left
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
