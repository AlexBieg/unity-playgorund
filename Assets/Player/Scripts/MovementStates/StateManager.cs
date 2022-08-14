using System.Collections.Generic;
using System.Linq;

public enum StateType
{
    GroundState,
    JumpState,
    FallingState,
    LedgeGrabState,
    WallRunState,
    VaultState,
    BalanceState
}

public class StateManager {
    private AbstractMovementState _activeState;
    public StateType ActiveStateType;

    private readonly Dictionary<StateType, AbstractMovementState> _stateMap;

    public StateManager(MovementInput input) {
        // Set up all the states
        _stateMap = new Dictionary<StateType, AbstractMovementState>
        {
            { StateType.GroundState, new GroundState(input) },
            { StateType.JumpState, new JumpState(input) },
            { StateType.FallingState, new FallingState(input) },
            { StateType.LedgeGrabState, new LedgeGrabState(input) },
            { StateType.WallRunState, new WallRunState(input) },
            { StateType.VaultState, new VaultState(input) },
            { StateType.BalanceState, new BalanceState(input) }
        };


        SwitchToState(StateType.GroundState);
    }

    private AbstractMovementState GetStateByStateType(StateType state)
    {
        return _stateMap[state];
    }

    private bool CanActivateState(StateType state)
    {
        return GetStateByStateType(state).CanActivate();
    }

    private void SwitchToState(StateType stateType) {
        _activeState?.TearDown();

        var nextState = GetStateByStateType(stateType);
        nextState.Initialize();
        _activeState = nextState;
        ActiveStateType = stateType;
    }

    public void UpdateStateCasts()
    {
        _activeState.UpdateCasts();
        
        foreach (var stateType in _activeState.NeighborStates)
        {
            GetStateByStateType(stateType).UpdateCasts();
        }
    }

    public void Update() {
        var newStates = _activeState.GetAvailableTransitions();

        foreach (var stateType in newStates.Where(CanActivateState))
        {
            SwitchToState(stateType);
        }

        _activeState.Update();
        _activeState.UpdateAnimator();
    }

    public void DebugGizmos()
    {
        _activeState.DebugGizmos();
    }
}