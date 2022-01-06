public class Musashi : Character
{
    protected override void SetupStates ()
    {
        _IdleState = new MusashiIdle(this);
        _RunningState = new MusashiRunning(this);
        _AscendingState = new MusashiAscending(this);
        _FallingState = new MusashiFalling(this);
        _JabAction = new MusashiJab(this);
        _HitState = new MusashiHit(this);
        _DashAbility = new MusashiDash(this);

        _CurrentState = _IdleState;
        _CurrentState.Enter(null, 0);
    }

    protected override string GetCharacterName => "Musashi";
}