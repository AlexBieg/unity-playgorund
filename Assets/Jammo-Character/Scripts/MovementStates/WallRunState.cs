
using System.Collections.Generic;
using UnityEngine;

public class WallRunState : AbstractMovementState
{
    public WallRunState(MovementInput input) : base(input) {}

    // public override StateType? GetNextState()
    // {
    //     if (_input.isGrounded)
    //     {
    //         return StateType.GROUND_STATE;
    //     }
    //
    //     if (_input.JumpInput.triggered) {
    //         _input.moveVector.x = _input.jumpVel;
    //         return StateType.JUMP_STATE;
    //     }
    //
    //     // if (_input.moveVector.y < 0 && PlayerCanGrabLedge())
    //     // {
    //     //     return StateType.LEDGE_GRAB_STATE;
    //     // }
    //
    //
    //     // TODO: Keep track of which wall we are running on
    //     if (!PlayerCanWallRun()) {
    //         return StateType.FALLING_STATE;
    //     }
    //
    //     return null;
    // }

    public override IEnumerable<StateType> GetAvailableTransitions()
    {
        return NeighborStates;
    }

    public override void Initialize()
    {
        // _input.prevVertVel = _input.jumpVel;
    }

    public override void Update()
    {
        float localGravity = Input.gravity * Input.wallRunGravityMultiplier;

        Input.HandleGravity(localGravity);
        Input.HandleMove();
    }

    public override void TearDown() { }
}