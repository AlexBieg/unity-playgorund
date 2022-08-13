
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Animations;

public class LedgeGrabState : AbstractMovementState
{
    private bool _gettingUp;
    private bool _finishedGettingUp;
    private Vector3 _shimmyMovement = Vector3.zero;
    private readonly List<StateType> _noDropNeighborStates;

    public LedgeGrabState(MovementInput input) : base(input)
    {
        NeighborStates.Add(StateType.JumpState);
        NeighborStates.Add(StateType.FallingState);
        NeighborStates.Add(StateType.GroundState);

        _noDropNeighborStates =
            new List<StateType>(NeighborStates.Where(stateType => stateType != StateType.FallingState));
    }

    public override bool CanActivate()
    {
        var isFalling = Input.moveVector.y <= 0;
        var foundLedge = Input.foundLedge;


        return isFalling && foundLedge;
    }
    
    
    public override IEnumerable<StateType> GetAvailableTransitions()
    {
        if (Vector2.Dot(Input.MoveInput, Vector2.down) > 0.99 || _finishedGettingUp)
        {
            return NeighborStates;
        }

        return _noDropNeighborStates;
    }


    public override void Initialize()
    {
        Input.animator.SetBool(AnimationHashes.isHanging, true);
        _gettingUp = false;
        _finishedGettingUp = false;


        // Reset velocities
        Input.prevVertVel = 0;
        Input.moveVector.x = 0;
        Input.moveVector.z = 0;
        Input.moveVector.y = 0;


    }

    public override void Update()
    {
        Vector2 movement = Input.MoveInput;

        bool startGettingUp = Vector2.Dot(movement, Vector2.up) > 0.99 && !Input.foundLedgeMountBlocker;

        if (startGettingUp)
        {
            Input.animator.SetBool(AnimationHashes.isClimbingUp, true);
        }

        if (_gettingUp || startGettingUp)
        {
            _gettingUp = true;
            var transform = Input.transform;
            
            Vector3 footPos = transform.position;
            footPos.y -= 1.8f;
            
             _finishedGettingUp = !Physics.Raycast(footPos, Input.transform.forward, 1);

            Input.desiredMoveDirection = (_finishedGettingUp ? Vector3.zero : Vector3.up) + transform.forward;
            
            Input.moveVector = Input.desiredMoveDirection * Input.shimmyVelocity;

            return;
        }

        Vector3 _ledgeFaceNormal = Input.wallHit.normal;
        Vector3 _rightShimmy = Vector3.Cross(_ledgeFaceNormal, Vector3.up).normalized;
        Input.transform.rotation = Quaternion.Slerp(Input.transform.rotation, Quaternion.LookRotation(-_ledgeFaceNormal), Input.desiredRotationSpeed / 2);

        var camForward = Input.cam.transform.forward;
        var camRight = Input.cam.transform.right;

        camForward.y = 0f;
        camRight.y = 0f;

        camForward.Normalize();
        camRight.Normalize();

        Input.desiredMoveDirection = camForward * movement.y + camRight * movement.x;
        Input.desiredMoveDirection.Normalize();

        Debug.DrawRay(Input.transform.position, Input.desiredMoveDirection * 2, Color.cyan);


        _shimmyMovement = _rightShimmy * (Vector3.Dot(_rightShimmy, Input.desiredMoveDirection) * Input.shimmyVelocity);
        Input.moveVector = _shimmyMovement + -_ledgeFaceNormal * Input.velocity;

        if (Input.transform.position.y + 1.5 > Input.ledgeHit.point.y) {
            Input.HandleGravity(Input.gravity * 2);
        }
    }

    public override void UpdateAnimator()
    {
        float dot = Vector3.Dot(Input.transform.right, Input.desiredMoveDirection);

        float mag = _shimmyMovement.magnitude / Input.shimmyVelocity;
        float magWithDirection = dot > 0 ? mag : -mag;
        Input.animator.SetFloat(AnimationHashes.HangMovement, magWithDirection);
        
    }

    public override void TearDown()
    {
        Input.animator.SetBool(AnimationHashes.isHanging, false);
        Input.animator.SetBool(AnimationHashes.isClimbingUp, false);
    }


}