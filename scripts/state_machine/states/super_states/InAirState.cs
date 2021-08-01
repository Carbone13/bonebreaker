using Bonebreaker.Inputs;
using Bonebreaker.Player;

namespace Bonebreaker.StateMachine
{
    public class InAirState : State
    {
        public InAirState (Character owner) : base(owner) => Owner = owner;

        protected override void _Tick (sfloat delta, uint frame, InputState input)
        {
            Owner.Velocity.Y += Owner.Gravity * delta;
            Owner.Move(delta);
        }

        protected override void _CheckExitConditions (InputState input)
        {
            base._CheckExitConditions(input);
            
            if (Owner.IsGrounded)
                Owner.EnterState(Owner.IdleState);
        }
    }
}