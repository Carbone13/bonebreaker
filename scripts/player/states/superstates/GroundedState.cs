using Bonebreaker.Inputs;
using Godot;

public class GroundedState : State
{
    protected override void _Enter (State previous)
    {
        Owner.Velocity = new sfloat2(Owner.Velocity.X, sfloat.Zero);
    }

    protected override State _ShouldExit (InputState input)
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
}