using Bonebreaker.Inputs;

public class RunningState : GroundedState
{
    protected override State _ShouldExit (InputState input, int tick)
    {
        State baseOpinion = base._ShouldExit(input, tick);

        if (baseOpinion == null)
        {
            if (input.Joystick.x == 0)
            {
                return Owner._IdleState;
            }
                
        }

        return baseOpinion;
    }

    protected override void _Tick (int frame, sfloat delta, InputState input)
    {
        base._Tick(frame, delta, input);
        Owner.Velocity += 
            new sfloat2(delta * (sfloat)input.Joystick.x * (sfloat)Owner.Stats.Acceleration, Owner.Velocity.Y);
        Owner.Velocity =
            new sfloat2(sfloat.Clamp(Owner.Velocity.X,  -(sfloat)Owner.Stats.MoveSpeed, (sfloat)Owner.Stats.MoveSpeed),
                Owner.Velocity.Y);
    }

    protected override void _Animate ()
    {
        if (Owner.Orientation == Orientation.Left)
        {
            Owner.Animator.Play("running_l");
        }
        else
        {
            Owner.Animator.Play("running_r");
        }
    }

    public override string ToString ()
    {
        return "Running";
    }

    public RunningState (Character owner) : base(owner)
    {
    }
}