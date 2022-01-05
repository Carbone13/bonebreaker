public class ActionState : State
{
    protected int lastEnteredTick;
    protected bool shouldExit;

    public int nextAllowedTick;
    
    public ActionState (Character owner) : base(owner) { }
}