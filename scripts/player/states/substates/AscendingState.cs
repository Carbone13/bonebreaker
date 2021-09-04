using Bonebreaker.Inputs;
using Godot;

public class AscendingState : InAirState
{
    protected override void _Enter (State previous)
    {
        Owner.Velocity = new sfloat2(Owner.Velocity.X, -Owner.Stats.JumpVelocity);
    }

    protected override State _ShouldExit (InputState input)
    {
        State baseOpinion = base._ShouldExit(input);

        if (baseOpinion == null)
        {
            if (Owner.Velocity.Y > sfloat.Zero)
                return Owner._FallingState;
        }

        return baseOpinion;
    }

    public override string ToString ()
    {
        return "Ascending";
    }
}
