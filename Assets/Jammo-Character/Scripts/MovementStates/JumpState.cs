
using System.Collections.Generic;
using Animations;

public class JumpState : AbstractMovementState
{
    public JumpState(MovementInput input) : base(input)
    {
        NeighborStates.Add(StateType.GroundState);
        NeighborStates.Add(StateType.LedgeGrabState);
    }

    public override bool CanActivate()
    {
        return Input.JumpInput.triggered;
    }
    
    public override IEnumerable<StateType> GetAvailableTransitions()
    {
        return NeighborStates;
    }


    public override void Initialize() {
        Input.prevVertVel = Input.jumpVel;
        Input.animator.SetBool(AnimationHashes.isJumping, true);
    }

    public override void Update()
    {
        float localGravity = Input.gravity;

        if (Input.prevVertVel < 0 || !Input.JumpInput.inProgress)
        {
            localGravity *= Input.fallGravityMultiplier;
        }

        Input.HandleGravity(localGravity);
        Input.HandleMove();
    }

    public override void TearDown()
    {
        Input.animator.SetBool(AnimationHashes.isJumping, false);
    }
}