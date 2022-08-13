using System.Collections.Generic;
using UnityEngine;

public class BalanceState : AbstractMovementState
{
    private const float BeamWidth = 0.2f;
    private const float BeamHeight = 1.6f;

    private bool _hitLeft;
    private bool _hitRight;

    public BalanceState(MovementInput input) : base(input)
    {
        NeighborStates.Add(StateType.JumpState);
        NeighborStates.Add(StateType.GroundState);
        NeighborStates.Add(StateType.FallingState);
    }

    public override void UpdateCasts()
    {
        var transform = Input.transform;
        var right = transform.right;
        var pos = transform.position;

        _hitLeft = Physics.Raycast(pos + -BeamWidth * right, Vector3.down, BeamHeight);
        _hitRight = Physics.Raycast(pos + BeamWidth * right, Vector3.down, BeamHeight);
        
        Debug.DrawRay(pos + -BeamWidth * right, Vector3.down * BeamHeight, Color.blue);
        Debug.DrawRay(pos + BeamWidth * right, Vector3.down * BeamHeight, Color.blue);
    }

    public override bool CanActivate()
    {
        return Input.isGrounded && !_hitLeft && !_hitRight;
    }

    
    public override IEnumerable<StateType> GetAvailableTransitions()
    {
        return NeighborStates;
    }

    public override void Initialize()
    {
    }

    public override void Update()
    {
        Input.HandleGroundMove();
    }

    public override void TearDown()
    {
        
    }
}