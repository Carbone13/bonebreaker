using Bonebreaker.Inputs;

public class GroundedState : State
{
    protected override void _Enter (State previous, int tick)
    {
        Owner.Velocity = new sfloat2(Owner.Velocity.X, sfloat.Zero);
    }

    protected override void _Tick (int frame, sfloat delta, InputState input)
    {
        if (input.Fall && Owner.GroundTag == "Jump-Through")
        {
            Owner.IsGrounded = false;
            Owner.AddToPosition(new sfloat2(sfloat.Zero, sfloat.One));
        }

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
        if (!Owner.IsGrounded)
        {
            return Owner._FallingState;
        }
        
        if (Owner.Velocity.Y != sfloat.Zero)
        {
            if (Owner.Velocity.Y > sfloat.Zero)
                return Owner._FallingState;
            if (Owner.Velocity.Y < sfloat.Zero)
                return Owner._AscendingState;
        }

        if (input.Jump)
        {
            Owner.IsGrounded = false;
            return Owner._AscendingState;
        }
        


        return null;
    }

    public GroundedState (Character owner) : base(owner)
    {
    }
}