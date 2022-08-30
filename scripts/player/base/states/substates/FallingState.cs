using Bonebreaker.Inputs;

public class FallingState : InAirState
{
    public override string ToString ()
    {
        return "Falling";
    }

    protected override void _Tick (int frame, sfloat delta, InputState input)
    {
        Owner.Velocity = new sfloat2((sfloat)Owner.Stats.MoveSpeed * (sfloat)input.Joystick.x * (sfloat)Owner.Stats.InAirDamping,
            Owner.Velocity.Y + Owner.Stats.Gravity * delta * (input.Fall ? (sfloat)Owner.Stats.FastFallingMultiplier : sfloat.One));
        
        if (input.Joystick.x > 0)
        {
            Owner.Orientation = Orientation.Right;
        }
        if (input.Joystick.x < 0)
        {
            Owner.Orientation = Orientation.Left;
        }
    }

    protected override void _Animate ()
    {
        if (Owner.Orientation == Orientation.Left)
        {
            Owner.Animator.Play("falling_l");
        }
        else
        {
            Owner.Animator.Play("falling_r");
        }
    }

    public FallingState (Character owner) : base(owner)
    {
    }
}
