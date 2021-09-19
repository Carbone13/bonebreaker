using Bonebreaker.Inputs;
using Godot;
using Godot.Collections;

public class JabAction : ActionState
{
    private int jabIndex;
    private int lastEnteredTick;
    private bool shouldExit;

    public override void _Init ()
    {
        Owner.Animator.Connect("animation_finished", this, nameof(AnimationFinished));
    }

    protected override void _Enter (State previous, int tick)
    {
        Owner.Velocity = sfloat2.Zero;
        
        if (tick - lastEnteredTick > Owner.Stats.JabResetTicks)
        {
            jabIndex = 0;
        }
        
        jabIndex++;
        if (jabIndex > Owner.Stats.JabCount)
        {
            jabIndex = 1;
        }
        
        lastEnteredTick = tick;
        
        if (Owner.Orientation == Orientation.Left)
        {
            Owner.Animator.Play("jab_" + jabIndex + "_l");
        }
        else
        {
            Owner.Animator.Play("jab_" + jabIndex + "_r");
        }
    }

    private void AnimationFinished (string name)
    {
        if(name.Contains("jab_") && Owner._CurrentState == Owner._JabAction)
            shouldExit = true;
    }

    protected override State _ShouldExit (InputState input, int tick)
    {
        if (shouldExit)
        {
            shouldExit = false;
            return Owner._IdleState;
        }

        return base._ShouldExit(input, tick);

    }

    
    public override string ToString ()
    {
        return "Jab Action";
    }

    public override Dictionary _Serialize ()
    {
        return new Dictionary
        {
            { "jab_index", jabIndex },
            { "last_entered_tick", lastEnteredTick },
            { "should_exit", shouldExit ? "1" : "0" }
        };
    }

    public override void _Deserialize (Dictionary state)
    {
        jabIndex = (int)state["jab_index"];
        lastEnteredTick = (int)state["last_entered_tick"];
        shouldExit = (string)state["should_exit"] == "1";
    }

    public JabAction (Character owner) : base(owner)
    {
    }
}
