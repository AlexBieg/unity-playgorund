
using System.Collections.Generic;
using UnityEngine;

public class FallingState : AbstractMovementState
{
    private float _coyoteTime = 0.2f;
    private float _elapsedTime;

    private readonly List<StateType> _noJumpNeighbors;

    public FallingState(MovementInput input) : base(input)
    {
        NeighborStates.Add(StateType.GroundState);
        NeighborStates.Add(StateType.LedgeGrabState);
        NeighborStates.Add(StateType.JumpState);

        _noJumpNeighbors = new List<StateType> { StateType.LedgeGrabState, StateType.GroundState };
    }

    public override bool CanActivate()
    {
        return !Input.isGrounded;
    }

    public override IEnumerable<StateType> GetAvailableTransitions()
    {
        return _elapsedTime > _coyoteTime ? _noJumpNeighbors : NeighborStates;
    }

    public override void Initialize(){
        Input.prevVertVel = -1;
        _elapsedTime = 0;
    }

    public override void Update()
    {
        _elapsedTime += Time.deltaTime;
        float localGravity = Input.gravity * Input.fallGravityMultiplier;

        Input.HandleGravity(localGravity);
        Input.HandleMove();
    }

    public override void TearDown() { }
}