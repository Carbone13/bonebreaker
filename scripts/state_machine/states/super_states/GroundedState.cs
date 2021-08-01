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
            Owner.CheckIfGrounded();
            CheckExitConditions(Owner.GetInput());
        }

        protected override void _Tick (sfloat delta, uint frame, InputState input)
        {
            Owner.LastGroundedTime = frame;

            if (input.Jump)
            {
                GD.Print("ju");
                Owner.IsGrounded = false;
                Owner.Velocity.Y = Owner.JumpForce;
                Owner.Move(delta);
                Owner.EnterState(Owner.AscendingState);
            }
        }

        protected override void _CheckExitConditions (InputState input)
        {
            base._CheckExitConditions(input);
            
            if (!Owner.IsGrounded)
                Owner.EnterState(Owner.FallingState);
        }
    }
}