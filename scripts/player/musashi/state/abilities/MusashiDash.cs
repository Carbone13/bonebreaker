using Bonebreaker.Inputs;
using Godot.Collections;

public class MusashiDash : ActionState
{
    public int endTick;

    protected override void _Enter (State previous, int tick)
    {
        lastEnteredTick = tick;
        endTick = tick + Owner.Stats.DashDuration;
        
        if (Owner.Orientation == Orientation.Left)
        {
            Owner.Velocity = new sfloat2(-(sfloat)Owner.Stats.DashSpeed, Owner.Velocity.Y);
        }
        else
        {
            Owner.Velocity = new sfloat2((sfloat)Owner.Stats.DashSpeed, Owner.Velocity.Y);
        }

        nextAllowedTick = tick + Owner.Stats.DashCooldownTicks;
    }

    protected override void _Tick (int frame, sfloat delta, InputState input)
    {
        if (shouldExit) return;
        
        base._Tick(frame, delta, input);
        
        if (frame >= endTick)
        {
            Owner.Velocity = sfloat2.Zero;
            shouldExit = true;
        }
        
        if (Owner.Orientation == Orientation.Left)
        {
            Owner.Velocity = new sfloat2(-(sfloat)Owner.Stats.DashSpeed, Owner.Velocity.Y);
        }
        else
        {
            Owner.Velocity = new sfloat2((sfloat)Owner.Stats.DashSpeed, Owner.Velocity.Y);
        }
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

    protected override void _Animate ()
    {
        if (Owner.Orientation == Orientation.Left)
        {
            Owner.Animator.Play("dash_l");
        }
        else
        {
            Owner.Animator.Play("dash_r");
        }
    }

    public override string ToString ()
    {
        return "Dash";
    }
    
    public override Dictionary _Serialize ()
    {
        return new Dictionary
        {
            { "should_exit", shouldExit ? "1" : "0" },
            { "last_entered_tick", lastEnteredTick },
            { "next_allowed_tick", nextAllowedTick },
            { "end_tick", endTick }
        };
    }

    public override void _Deserialize (Dictionary state)
    {
        lastEnteredTick = (int)state["last_entered_tick"];
        shouldExit = (string)state["should_exit"] == "1";
        nextAllowedTick = (int)state["next_allowed_tick"];
        endTick = (int)state["end_tick"];
    }
    
    public MusashiDash (Character owner) : base(owner)
    {
    }
}
