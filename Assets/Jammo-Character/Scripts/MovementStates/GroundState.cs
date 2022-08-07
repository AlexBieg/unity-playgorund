using System.Collections.Generic;
using UnityEngine;
using Animations;

public class GroundState : AbstractMovementState {
    private float _minGroundedTime = 1f;
    private float _elapsedTime;

    public GroundState(MovementInput input) : base(input)
    {
        NeighborStates.Add(StateType.JumpState);
        NeighborStates.Add(StateType.FallingState);
        NeighborStates.Add(StateType.VaultState);
    }
    
    public override bool CanActivate()
    {
        return Input.isGrounded;
    }

    public override IEnumerable<StateType> GetAvailableTransitions()
    {
        return NeighborStates;
    }

    public override void Initialize() {
        Input.AdjustMove(Vector3.down);
        Input.prevVertVel = -1;
        Input.animator.SetBool(AnimationHashes.isGrounded, true);
        _elapsedTime = 0;
    }
    public override void Update()
    {
        _elapsedTime += Time.deltaTime;

        if (_elapsedTime >= _minGroundedTime)
        {
            _elapsedTime = 0;
            Input.lastGroundedPos = Input.transform.position;
        }
        
        Input.HandleMove();
    }
    
    public override void UpdateAnimator()
    {
        Vector3 moveVector = Input.moveVector;
        float currentSpeed = new Vector2(moveVector.x, moveVector.z).magnitude;
        float maxSpeed = Input.velocity * Input.sprintMultiplier;
        Input.animator.SetFloat(AnimationHashes.Speed, currentSpeed / maxSpeed);
    }

    public override void TearDown()
    {
        Input.animator.SetBool(AnimationHashes.isGrounded, false);
    }


}