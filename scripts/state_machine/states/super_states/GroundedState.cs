using Bonebreaker.Inputs;
using Bonebreaker.Player;
using Godot;

namespace Bonebreaker.StateMachine
{
    public class GroundedState : State
    {
        public GroundedState (Character owner) : base(owner) => Owner = owner;

        protected override void _Enter (State previousState)
        {
            base._Enter(previousState);
        }

        protected override void _CheckExitConditions (InputState input)
        {
            base._CheckExitConditions(input);
            
            if (!Owner.IsGrounded)
                Owner.EnterState(Owner.FallingState);
            
            if (input.Jump)
            {
                Owner.IsGrounded = false;
                Owner.Velocity.Y = -Owner.JumpForce;
                Owner.EnterState(Owner.AscendingState);
            }
        }
    }
}