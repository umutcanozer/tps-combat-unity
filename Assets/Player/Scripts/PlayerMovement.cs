using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintSpeed = 7f;
    [SerializeField] private float smoothTime = 0.05f;
    [SerializeField] private Transform cameraTransform;

    private Rigidbody _rb;
    private AnimationHandler _animatorHandler;
    private LockTarget _lockTarget;

    private float _verticalAxis;
    private float _horizontalAxis;
    
    private float _speed;
    private float _refVelocity;
    private bool _isSprinting;
    private bool _isAttacking;
    private Vector3 _moveDirection;

    private void OnEnable()
    {
        EventManager.OnSprintInputPerformed += OnSprintingStateChange;
        EventManager.OnMovementInputsPerformed += OnMovementSetAxis;
    }
    private void OnSprintingStateChange(bool sprintingState)
    {
        _isSprinting = sprintingState;
        if(_lockTarget.IsLocked) _animatorHandler.SetLockedSprintParameters(_isSprinting ? 1f : 0.4f, _isSprinting ? 1f : 0.4f);
    }

    private void OnMovementSetAxis(Vector2 axis)
    {
        _verticalAxis = axis.y;
        _horizontalAxis = axis.x;
    }

    private void OnDisable()
    {
        EventManager.OnSprintInputPerformed -= OnSprintingStateChange;
        EventManager.OnMovementInputsPerformed -= OnMovementSetAxis;
    }

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _animatorHandler = AnimationHandler.GetInstance(GetComponent<Animator>());
        _lockTarget = GetComponent<LockTarget>();
    }
    
    void FixedUpdate()
    {
        if (_animatorHandler.AttackAdjustment()) return;
        HandleMovement();
    }

    private void HandleMovement()
    {
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        forward.y = 0f; 
        right.y = 0f;   
        
        HandleMovementSpeed(_verticalAxis, _horizontalAxis, forward, right);

        if (!_lockTarget.IsLocked)
        {
            HandleMovementForNormalState(_moveDirection);
            RotateRelativeInput(_verticalAxis, _horizontalAxis);
        }
        else
            HandleMovementForLockedState(_verticalAxis, _horizontalAxis, forward, right);
    }

    private void HandleMovementSpeed(float verticalInput, float horizontalInput, Vector3 camForward, Vector3 camRight)
    {
        _moveDirection = (camForward * verticalInput + camRight * horizontalInput).normalized;
        _speed = _isSprinting ? sprintSpeed : moveSpeed;
        _moveDirection *= _speed;

        _rb.velocity = new Vector3(_moveDirection.x, _rb.velocity.y, _moveDirection.z) * Time.fixedDeltaTime;
    }

    private void HandleMovementForNormalState(Vector3 moveDirection)
    {
        _animatorHandler.UpdateMovementParameters(moveDirection.magnitude);
    }

    private void HandleMovementForLockedState(float verticalInput, float horizontalInput, Vector3 camForward, Vector3 camRight)
    {
        _animatorHandler.UpdateLockedMovementParameters(verticalInput, horizontalInput);
        RotateTowardsTarget(_lockTarget.TargetObject.transform.position);
    }

    private void RotateTowardsTarget(Vector3 targetPosition)
    {
        Vector3 targetDirection = targetPosition - transform.position;
        targetDirection.y = 0;

        transform.rotation = Quaternion.Euler(0f, HandleQuaternion(targetDirection), 0f);
    }
    
    private void RotateRelativeInput(float verticalInput, float horizontalInput)
    {
        float cameraYaw = cameraTransform.eulerAngles.y;
        
        Vector3 targetDirection = new Vector3(horizontalInput, 0f, verticalInput);
        targetDirection = Quaternion.Euler(0, cameraYaw, 0) * targetDirection;
        targetDirection = Vector3.ClampMagnitude(targetDirection, 1);
        
        if (targetDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.Euler(0f, HandleQuaternion(targetDirection), 0f);
        }
    }

    private float HandleQuaternion(Vector3 target)
    {
        float targetAngle = Mathf.Atan2(target.x, target.z) * Mathf.Rad2Deg;
        float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _refVelocity, smoothTime);

        return smoothAngle;
    }
}
