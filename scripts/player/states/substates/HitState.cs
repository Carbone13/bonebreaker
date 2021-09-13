using Bonebreaker.Inputs;
using Godot;
using Godot.Collections;

public class HitState : ActionState
{
    private int exitTick;
    private bool shouldExit;
    
    public override void _Init ()
    {
        Owner.Animator.Connect("animation_finished", this, nameof(AnimationFinished));
    }

    protected override void _Enter (State previous, int tick)
    {
        shouldExit = false;
        GD.Print("hit");
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
}