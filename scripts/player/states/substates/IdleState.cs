using Bonebreaker.Inputs;
using Godot;

public class IdleState : GroundedState
{
    protected override void _Enter (State previous)
    {
        base._Enter(previous);
        Owner.Velocity = new sfloat2(sfloat.Zero, Owner.Velocity.Y);
    }

    protected override State _ShouldExit (InputState input)
    {
        State baseOpinion = base._ShouldExit(input);

        if (baseOpinion == null)
        {
            if (input.Joystick.x != 0)
                return Owner._RunningState;
        }

        return baseOpinion;
    }
    
    public override string ToString ()
    {
        return "Idle";
    }
}
