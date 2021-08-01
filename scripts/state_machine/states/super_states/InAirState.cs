using Bonebreaker.Inputs;
using Bonebreaker.Player;
using Godot;

namespace Bonebreaker.StateMachine
{
    public class InAirState : State
    {
        public InAirState (Character owner) : base(owner) => Owner = owner;

        protected override void _Tick (sfloat delta, uint frame, InputState input)
        {
            Owner.Velocity.Y += Owner.Gravity * delta;
            int dir = input.xInput;
            Owner.Velocity.X = (sfloat)dir * (sfloat)Owner.MOVE_SPEED;
        }

        protected override void _CheckExitConditions (InputState input)
        {
            base._CheckExitConditions(input);

            if (Owner.IsGrounded)
                Owner.EnterState(Owner.IdleState);
        }
    }
}