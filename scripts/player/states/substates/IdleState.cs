using Bonebreaker.Inputs;
using Godot;

public class IdleState : GroundedState
{
    protected override void _Enter (State previous, int tick)
    {
        base._Enter(previous, tick);
        Owner.Velocity = new sfloat2(sfloat.Zero, Owner.Velocity.Y);
    }
    
    protected override State _ShouldExit (InputState input, int tick)
    {
        State baseOpinion = base._ShouldExit(input, tick);

        if (baseOpinion == null)
        {
            if (input.Joystick.x != 0)
                return Owner._RunningState;
        }

        return baseOpinion;
    }

    protected override void _Animate ()
    {
        if (Owner.Orientation == Orientation.Left)
        {
            Owner.Animator.Play("idle_l");
        }
        else
        {
            Owner.Animator.Play("idle_r");
        }
    }

    public override string ToString ()
    {
        return "Idle";
    }
}
