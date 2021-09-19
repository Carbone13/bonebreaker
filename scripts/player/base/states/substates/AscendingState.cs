using Bonebreaker.Inputs;
using Godot;

public class AscendingState : InAirState
{
    protected override void _Enter (State previous, int tick)
    {
        Owner.Velocity = new sfloat2(Owner.Velocity.X, -Owner.Stats.JumpVelocity);
    }

    protected override State _ShouldExit (InputState input, int tick)
    {
        State baseOpinion = base._ShouldExit(input, tick);

        if (baseOpinion == null)
        {
            if (Owner.Velocity.Y > sfloat.Zero)
                return Owner._FallingState;
        }

        return baseOpinion;
    }

    protected override void _Animate ()
    {
        if (Owner.Orientation == Orientation.Left)
        {
            Owner.Animator.Play("ascending_l");
        }
        else
        {
            Owner.Animator.Play("ascending_r");
        }
    }

    public override string ToString ()
    {
        return "Ascending";
    }

    public AscendingState (Character owner) : base(owner)
    {
    }
}
