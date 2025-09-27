using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(Rigidbody))]
public class TestPawnController : MonoBehaviour
{
    [Header("Input")] public InputActionReference moveAction;
    public InputActionReference dashAction;


    [Header("Movement")] public float moveSpeed = 6f;
    public float dashForce = 18f;
    public float dashCooldown = 0.35f;


    Rigidbody _rb; Vector3 _moveDir; bool _dashRequested; float _nextDashTime;


    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.constraints = RigidbodyConstraints.FreezeRotation;
    }


    void OnEnable()
    {
        moveAction.action.Enable();
        dashAction.action.Enable();
        dashAction.action.performed += OnDash;
    }


    void OnDisable()
    {
        dashAction.action.performed -= OnDash;
        moveAction.action.Disable();
        dashAction.action.Disable();
    }


    void Update()
    {
        Vector2 mv = moveAction.action.ReadValue<Vector2>();
        // Convert 2D input to world XZ (2.5D). Forward is +Z.
        _moveDir = new Vector3(mv.x, 0f, mv.y).normalized;


        // Visual facing (optional)
        if (_moveDir.sqrMagnitude > 0.001f)
            transform.forward = Vector3.Lerp(transform.forward, _moveDir, Time.deltaTime * 12f);
    }


    void FixedUpdate()
    {
        // Continuous move
        Vector3 targetVel = _moveDir * moveSpeed;
        Vector3 vel = _rb.linearVelocity;
        vel.x = targetVel.x; vel.z = targetVel.z;
        _rb.linearVelocity = vel;


        if (_dashRequested)
        {
            _dashRequested = false;
            if (Time.time >= _nextDashTime)
            {
                Vector3 dir = (_moveDir.sqrMagnitude > 0.001f) ? _moveDir : transform.forward;
                _rb.AddForce(dir.normalized * dashForce, ForceMode.VelocityChange);
                _nextDashTime = Time.time + dashCooldown;
            }
        }
    }


    void OnDash(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) _dashRequested = true; // handled in FixedUpdate
    }
}