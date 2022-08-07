using System;
using UnityEngine;



public class PlayerAnimator : MonoBehaviour
{
    [Header("General")]
    public LayerMask layerMask;
    public Vector3 centerOfMassGround;
    
    [Header("Walk Cycle")] public float stepHeight = 1;
    public float stepSize = 1;
    public float stepSpeed = 1;
    public bool moveLeftLeg = true;
    public Vector3 nextFootPos = Vector3.zero;
    public Vector3 prevFootPos = Vector3.zero;
    public float footMoveAmount = 0;
    public float stepTime = 1;
    public float stepTimer = 1.1f;
    public Vector2 standingRect = Vector2.one;

    [Header("Legs")] public GameObject LeftLegTarget;
    public float leftAngle;

    public GameObject RightLegTarget;
    public float rightAngle = 3.14159f;
    public float LegSpacing = 0.3f;
    public float footHeight = 0.05f;
    public float feetLength = 0.1f;

    [Header("Arms")] public GameObject LeftArmTarget;
    public GameObject RightArmTarget;
    public float ArmSideOffset = 1;
    public float ArmHeight = 1;

    [Header("Spine")] public GameObject LeanTarget;
    public float LeanWeight = 1;

    [Header("Hips")] public GameObject HipsTarget;
    public float BounceWeight = 0.1f;
    public float hipsOffset = 0;
    private MovementInput _input;

    private RaycastHit leftGroundHit;
    private RaycastHit rightGroundHit;

    // Start is called before the first frame update
    private void Start()
    {
        _input = GetComponent<MovementInput>();
    }

    // Update is called once per frame
    private void Update()
    {
        //UpdateLean();
        UpdateLegs();
        // UpdateLegsV2();
        //UpdateArms();
        UpdateHips();
    }

    private void FixedUpdate()
    {
        Physics.Raycast(LeftLegTarget.transform.position + 2 * stepHeight * Vector3.up, Vector3.down,
            out leftGroundHit, layerMask);
        Physics.Raycast(RightLegTarget.transform.position + 2 * stepHeight * Vector3.up, Vector3.down,
            out rightGroundHit, layerMask);
        Physics.Raycast(_input.transform.position, Vector3.down, out var COMHit, layerMask);

        centerOfMassGround = COMHit.point;
    }

    private void UpdateHips()
    {
        // var bounceAmount = Math.Max(Mathf.Sin(leftAngle), Mathf.Sin(rightAngle));
        // var pos = _input.transform.position;
        // var yVal = pos.y + hipsOffset;
        // var leftPoint = LeftLegTarget.transform.position;
        // var rightPoint= RightLegTarget.transform.position;
        //
        // var heightDiffVector = leftPoint - rightPoint;
        // var hipRotation = Quaternion.FromToRotation(Vector3.right, heightDiffVector);
        //
        // HipsTarget.transform.rotation = hipRotation;
    }

    private void UpdateLegsV2()
    {
        // if (stepTimer > stepTime)
        // {
        //     footMoveAmount = 0;
        //     stepTimer = 0;
        //     moveLeftLeg = !moveLeftLeg;
        //
        //     var currentStepSize = stepSize; // * _input.moveVector.magnitude;
        //     var charTransform = _input.transform;
        //     var forwardRay = charTransform.forward * currentStepSize;
        //     var position = charTransform.position;
        //     var footLocationRay = position + forwardRay;
        //
        //     Physics.Raycast(footLocationRay, Vector3.down, out var nextFootLocation, 3f, layerMask);
        //     nextFootPos = nextFootLocation.point;
        //
        //     prevFootPos = moveLeftLeg ? LeftLegTarget.transform.position : RightLegTarget.transform.position;
        // }
        //
        //
        // var legTarget = moveLeftLeg ? LeftLegTarget : RightLegTarget;
        // // var currentPos =  Parabola(prevFootPos, nextFootPos, stepHeight, footMoveAmount);
        //
        // // legTarget.transform.position = currentPos;
        //
        // footMoveAmount += stepSpeed * Time.deltaTime;
        // stepTimer += Time.deltaTime;
    }

    private void UpdateLegs()
    {
        var vel = _input.desiredMoveDirection.magnitude;
        var forward = _input.transform.forward;

        var currentStepSize = stepSize * vel;
        var currentStepSpeed = stepSpeed * vel;
        var currentStepHeight = stepHeight * vel;

        var pos = _input.transform.position;
        pos.y -= 1.5f;
        var leftX = -Mathf.Cos(leftAngle) * currentStepSize;
        var leftY = Mathf.Sin(leftAngle) * currentStepHeight;

        var rightX = -Mathf.Cos(rightAngle) * currentStepSize;
        var rightY = Mathf.Sin(rightAngle) * currentStepHeight;

        var right = _input.transform.right;

        var rightOffset = pos + Vector3.up * rightY + forward * rightX + right * LegSpacing;
        var leftOffset = pos + Vector3.up * leftY + forward * leftX + right * -LegSpacing;

        leftOffset.y = (leftY > 0 ? Math.Max(leftOffset.y, leftGroundHit.point.y) : leftGroundHit.point.y) + footHeight;
        rightOffset.y = (rightY > 0 ? Math.Max(rightOffset.y, rightGroundHit.point.y) : rightGroundHit.point.y) + footHeight;

        LeftLegTarget.transform.position = leftOffset;
        RightLegTarget.transform.position = rightOffset;
        
        RightLegTarget.transform.rotation = Quaternion.FromToRotation(Vector3.up, rightGroundHit.normal) * Quaternion.LookRotation(forward, Vector3.up);
        LeftLegTarget.transform.rotation = Quaternion.FromToRotation(Vector3.up, leftGroundHit.normal) * Quaternion.LookRotation(forward, Vector3.up);

        leftAngle += currentStepSpeed * Time.deltaTime;
        rightAngle += currentStepSpeed * Time.deltaTime;
    }

    private void UpdateArms()
    {
        var right = _input.transform.right;

        var leftArmPos = RightLegTarget.transform.position + -ArmSideOffset * right;
        var rightArmPos = LeftLegTarget.transform.position + ArmSideOffset * right;

        leftArmPos.y = rightGroundHit.point.y + ArmHeight;
        rightArmPos.y = leftGroundHit.point.y + ArmHeight;


        LeftArmTarget.transform.position = leftArmPos;
        RightArmTarget.transform.position = rightArmPos;
    }

    private void UpdateLean()
    {
        var moveVector = _input.moveVector;
        Vector3 flatMoveVector = new(moveVector.x, 0, moveVector.z);
        var leanPos = _input.transform.position + Vector3.up * 1.5f;

        LeanTarget.transform.position = leanPos + flatMoveVector * LeanWeight;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(centerOfMassGround, new Vector3(0.1f, 0.1f, 0.1f));

        var rightPos = RightLegTarget.transform.position;
        var leftPos = LeftLegTarget.transform.position;
        Vector3 feetSquareCenter =
            Vector3.Lerp(leftPos, rightPos, 0.5f);
        feetSquareCenter.y = centerOfMassGround.y;
        Gizmos.DrawWireCube(feetSquareCenter, new Vector3(1, 0, 1));
    }
}