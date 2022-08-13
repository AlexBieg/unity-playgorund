
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Animator))]
public class MovementInput : MonoBehaviour
{
    [Header("Related Components")]
    public Camera cam;
    public CinemachineFreeLook vcam;
    public CharacterController controller;
    public Animator animator;
    private PlayerInput _input;

    [Header("State Machine")]
    public StateType ActiveState = StateType.GroundState;

    [Header("Movement Variables")]
    public float velocity;
    public float airAcceleration = 0.5f;
    public float shimmyVelocity = 5;
    public float sprintMultiplier;
    [Space]
    public float desiredRotationSpeed = 0.1f;
    public float maxAngleToCenter = 20f;
    public float jumpHeight = 1.0f;
    public float jumpTime = 0.5f;
    public float fallGravityMultiplier = 2.0f;
    public float wallRunGravityMultiplier = 0.5f;
    public float wallRunDistance = 2.0f;


    [Header("Movement Computed")]
    public float jumpVel;
	public float gravity;

    [Header("Movement Output")]
    public float prevVertVel;
    public Vector3 moveVector = Vector3.zero;
    public Vector3 desiredMoveDirection = Vector3.zero;
    public bool isGrounded = true;

    [field: Header("Player Input")]
    public InputAction JumpInput { get; private set; } = new();
    public InputAction SprintInput { get; private set; } = new();
    public Vector2 MoveInput { get; private set; }

    // [Header("State Management")]
    private StateManager _stateManager;

    [Header("Player View")]
    public RaycastHit wallHit;
    public bool foundWall;
    public RaycastHit ledgeHit;
    public bool foundLedge;
    public RaycastHit ledgeMountBlockerHit;
    public bool foundLedgeMountBlocker;

    [Header("Respawn")] public Vector3 lastGroundedPos;

    [Header("Debug")] public bool DrawGizmos;

    // Use this for initialization
    void Start()
    {
        cam = Camera.main;
        controller = GetComponent<CharacterController>();
        _input = GetComponent<PlayerInput>();
        animator = GetComponent<Animator>();

        vcam.m_RecenterToTargetHeading.m_enabled = false;

        SetupJump();

        moveVector = new Vector3(0, -1, 0);

        _stateManager = new StateManager(this);
        ActiveState = _stateManager.ActiveStateType;
    }

    public void FixedUpdate()
    {
        UpdateCasts();
        _stateManager.UpdateStateCasts();
    }

    private void UpdateCasts()
    {
        Vector3 p1 = transform.position + Vector3.down;
        Vector3 p2 = transform.position + Vector3.up;

        // Check for wall collision
        foundWall = Physics.CapsuleCast(p1, p2, 0.5f, transform.forward, out wallHit, 3);
        Debug.DrawRay(p1, transform.forward* 3, Color.red);
        Debug.DrawRay(p2, transform.forward* 3, Color.red);

        // If there is a wall check if there is a ledge
        if (foundWall)
        {
            Vector3 ledgeTopRayStart = wallHit.point + -wallHit.normal * 0.1f;
            ledgeTopRayStart.y = transform.position.y + 1.7f;

            foundLedge = Physics.Raycast(ledgeTopRayStart, Vector3.down, out ledgeHit, 3f);
            Debug.DrawRay(ledgeTopRayStart, Vector3.down * 3f, Color.blue);
        } else {
            foundLedge = false;
        }

        // if there is a ledge, check if it has clearance to get over it
        if (foundLedge) {
            Vector3 p1Clearance = p1 + Vector3.up * 3.2f;
            Vector3 p2Clearance = p2 + Vector3.up * 3.2f;

            foundLedgeMountBlocker = Physics.CapsuleCast(p1Clearance, p2Clearance, 0.5f, transform.forward, out ledgeMountBlockerHit, 4);
        } else {
            foundLedgeMountBlocker = false;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        isGrounded = controller.isGrounded;

        // Player input stuff
        UpdateInputs();

        // Camera Stuff
        UpdateCamera();

        // Update the state
        _stateManager.Update();
        ActiveState = _stateManager.ActiveStateType;

        // Actually move the character
        controller.Move(moveVector * Time.deltaTime);
    }

    private void UpdateInputs()
    {
        JumpInput = _input.actions["Jump"];
        SprintInput = _input.actions["Sprint"];
        MoveInput = _input.actions["Move"].ReadValue<Vector2>();
        
        var movement = MoveInput;

        var camTransform = cam.transform;
        var camForward = camTransform.forward;
        var camRight = camTransform.transform.right;

        camForward.y = 0f;
        camRight.y = 0f;

        camForward.Normalize();
        camRight.Normalize();

        desiredMoveDirection = camForward * movement.y + camRight * movement.x;
    }

    private void SetupJump()
    {
        float apexTime = jumpTime / 2;
        gravity = -2 * jumpHeight / Mathf.Pow(apexTime, 2);

        jumpVel = 2 * jumpHeight / apexTime;
    }
    
    private void UpdateCamera()
    {
        var angleDiff = Vector3.Angle(cam.transform.forward, transform.forward);

        if (MoveInput.magnitude > 0 && angleDiff < maxAngleToCenter)
        {
            vcam.m_RecenterToTargetHeading.m_enabled = true;
        }
        else
        {
            vcam.m_RecenterToTargetHeading.m_enabled = false;
        }
    }
    
    public void HandleGravity(float localGravity)
    {
        float oldVertVel = prevVertVel;
        prevVertVel += localGravity * Time.deltaTime;

        moveVector.y = (oldVertVel + prevVertVel) / 2;
    }

    public void HandleGroundMove(float velMultiplier = 1.0f)
    {
        var movement = MoveInput;
        var isSprinting = SprintInput.IsPressed();

        if (movement.magnitude > 0)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation,
                Quaternion.LookRotation(desiredMoveDirection), desiredRotationSpeed);
        }

        var newMoveVector = desiredMoveDirection * (velocity * velMultiplier);
        moveVector.x = newMoveVector.x;
        moveVector.z = newMoveVector.z;

        if (isSprinting)
        {
            moveVector.x *= sprintMultiplier;
            moveVector.z *= sprintMultiplier;
        }
    }

    public void HandleAirMove()
    {
        var movement = MoveInput;
        if (movement.magnitude > 0)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation,
                Quaternion.LookRotation(desiredMoveDirection), desiredRotationSpeed);
        }

        var desiredMoveVector = desiredMoveDirection * velocity;
        moveVector = Vector3.MoveTowards(moveVector, desiredMoveVector, airAcceleration);
    }

    public void AdjustMove(Vector3 adjustment)
    {
        moveVector += adjustment;
    }

    public void OnDrawGizmos()
    {
        if (!Application.isPlaying || !DrawGizmos) return;
        
        _stateManager.DebugGizmos();

        var trans= transform;
        
        if (foundWall)
        {
            Vector3 wallPoint = trans.position + trans.forward * wallHit.distance;
            Gizmos.DrawWireCube(wallPoint, new Vector3(1, 3, 1));
        }

        if (foundLedge)
        {
            Gizmos.DrawWireSphere(ledgeHit.point, 0.2f);
        }

        if (foundLedgeMountBlocker)
        {
            Vector3 clearancePoint = trans.position + trans.forward * ledgeMountBlockerHit.distance + Vector3.up * 3.1f;
            Gizmos.DrawWireCube(clearancePoint, new Vector3(1, 3, 1));
        }
    }
}
