
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VaultState : AbstractMovementState
{
    private readonly IEnumerable<StateType> _emptyNeighborStates = Enumerable.Empty<StateType>();
    private float _apexHeight;


    public VaultState(MovementInput input) : base(input)
    {
        NeighborStates.Add(StateType.FallingState);
    }

    public override IEnumerable<StateType> GetAvailableTransitions()
    {
        if (Input.transform.position.y >= _apexHeight)
        {
            return NeighborStates;
        }

        return _emptyNeighborStates;
    }

    public override bool CanActivate()
    {
        Vector3 wallNormal = Input.wallHit.normal;

        float angle = Vector3.Angle(-wallNormal, Input.transform.forward);

        return Input.foundLedge && !Input.foundLedgeMountBlocker && Input.SprintInput.IsPressed() && angle <=20;
    }

    public override void Initialize()
    {
        _apexHeight = Input.ledgeHit.point.y + 1.5f;
        Input.animator.SetBool("isVaulting", true);
    }

    public override void Update()
    {
        Vector3 newMoveVector = Input.moveVector + Vector3.up;
        newMoveVector.Normalize();

        Input.moveVector = newMoveVector * Input.velocity;
    }

    public override void TearDown()
    {
        Input.animator.SetBool("isVaulting", false);
    }
}