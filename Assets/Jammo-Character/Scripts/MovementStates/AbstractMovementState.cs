using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractMovementState
{
    protected readonly MovementInput Input;
    public readonly List<StateType> NeighborStates;

    protected AbstractMovementState(MovementInput input)
    {
        Input = input;
        NeighborStates = new List<StateType>();
    }

    public abstract IEnumerable<StateType> GetAvailableTransitions();
    public abstract void Initialize();
    public abstract void Update();
    public abstract void TearDown();

    public virtual bool CanActivate()
    {
        return true;
    }

    public virtual void DebugGizmos()
    {
    }

    public virtual void UpdateAnimator()
    {
    }

    public virtual void UpdateCasts()
    {
    }

    
}