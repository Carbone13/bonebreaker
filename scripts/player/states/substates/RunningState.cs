using Bonebreaker.Inputs;
using Godot;

public class RunningState : GroundedState
{
    protected override State _ShouldExit (InputState input)
    {
        State baseOpinion = base._ShouldExit(input);

        if (baseOpinion == null)
        {
            if (input.Joystick.x == 0)
                return Owner._IdleState;
        }

        return baseOpinion;
    }

    protected override void _Tick (int frame, sfloat delta, InputState input)
    {
        Owner.Velocity = 
            new sfloat2((sfloat)Owner.Stats.MoveSpeed * (sfloat)input.Joystick.x, Owner.Velocity.Y);
    }
    
    public override string ToString ()
    {
        return "Running";
    }
}