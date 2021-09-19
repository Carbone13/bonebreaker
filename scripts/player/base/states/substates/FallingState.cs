public class FallingState : InAirState
{
    public override string ToString ()
    {
        return "Falling";
    }
    
    protected override void _Animate ()
    {
        if (Owner.Orientation == Orientation.Left)
        {
            Owner.Animator.Play("falling_l");
        }
        else
        {
            Owner.Animator.Play("falling_r");
        }
    }

    public FallingState (Character owner) : base(owner)
    {
    }
}
