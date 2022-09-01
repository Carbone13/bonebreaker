using Bonebreaker.Inputs;
using Godot.Collections;

public class HitState : ActionState
{
    private new bool shouldExit;

    protected override void _Init ()
    {
        Owner.Animator.Connect("animation_finished", this, nameof(AnimationFinished));
    }

    protected override void _Exit (State next)
    {
        Owner.Velocity = new sfloat2(sfloat.Zero, Owner.Velocity.Y);
    }

    protected override void _Tick (int frame, sfloat delta, InputState input)
    {
        base._Tick(frame, delta, input);

        if (!Owner.IsGrounded)
        {
            Owner.Velocity = new sfloat2(Owner.Velocity.X, Owner.Velocity.Y + Owner.Stats.Gravity * delta);
        }
        
        Owner.Velocity = new sfloat2(Owner.Velocity.X * (sfloat)0.95f, Owner.Velocity.Y);
    }

    protected override void _Enter (State previous, int tick)
    {
        shouldExit = false;

        Owner.Velocity = new sfloat2((sfloat)(int)Owner.Orientation * -(sfloat)1 * (sfloat)60, Owner.Velocity.Y - (sfloat)80);
        
        if (Owner.Orientation == Orientation.Left)
        {
            Owner.Animator.Play("hit" + "_l");
        }
        else
        {
            Owner.Animator.Play("hit" + "_r");
        }
    }

    private void AnimationFinished (string name)
    {
        if(name.Contains("hit_") && Owner._CurrentState == this)
            shouldExit = true;
    }

    // TODO exit may "freeze"
    protected override State _ShouldExit (InputState input, int tick)
    {
        if (shouldExit)
        {
            shouldExit = false;
            _Exit(null);
            return Owner._IdleState;
        }
        
        return null;
    }
    
    public override Dictionary _Serialize ()
    {
        return new Dictionary
        {
            { "should_exit", shouldExit ? "1" : "0" }
        };
    }

    public override void _Deserialize (Dictionary state)
    {
        shouldExit = (string)state["should_exit"] == "1";
    }

    public HitState (Character owner) : base(owner)
    {
    }
}