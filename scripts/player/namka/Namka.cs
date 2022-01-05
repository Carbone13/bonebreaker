public class Namka : Character
{
    protected override void SetupStates ()
    {
        _IdleState = new NamkaIdle(this);
        _RunningState = new NamkaRunning(this);
        _AscendingState = new NamkaAscending(this);
        _FallingState = new NamkaFalling(this);
        _JabAction = new NamkaJab(this);
        _HitState = new NamkaHit(this);
        
        // empty
        _DashAbility = new ActionState(this);

        _CurrentState = _IdleState;
        _CurrentState.Enter(null, 0);
    }
}
