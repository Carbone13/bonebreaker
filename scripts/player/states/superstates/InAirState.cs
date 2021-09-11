using Bonebreaker.Inputs;
using Godot;

public class InAirState : State
{
    protected override void _Tick (int frame, sfloat delta, InputState input)
    {
        Owner.Velocity = new sfloat2((sfloat)Owner.Stats.MoveSpeed * (sfloat)input.Joystick.x * (sfloat)Owner.Stats.InAirDamping, Owner.Velocity.Y + Owner.Stats.Gravity * delta);
        
        if (input.Joystick.x > 0)
        {
            Owner.Orientation = Orientation.Right;
        }
        if (input.Joystick.x < 0)
        {
            Owner.Orientation = Orientation.Left;
        }
    }

    protected override State _ShouldExit (InputState input, int tick)
    {
        if (Owner.Velocity.Y == sfloat.Zero && Owner.IsGrounded)
        {
            if (input.Joystick.x == 0)
                return Owner._IdleState;
            else
                return Owner._RunningState;
        }

        return null;
    }
}
