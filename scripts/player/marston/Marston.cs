public class Marston : Character
{
    protected override void SetupStates ()
    {
        _IdleState = new MarstonIdle(this);
        _RunningState = new MarstonRunning(this);
        _AscendingState = new MarstonAscending(this);
        _FallingState = new MarstonFalling(this);
        _JabAction = new MarstonJab(this);
        _HitState = new MarstonHit(this);
        
        // empty
        _DashAbility = new ActionState(this);

        _CurrentState = _IdleState;
        _CurrentState.Enter(null, 0);
    }
}
