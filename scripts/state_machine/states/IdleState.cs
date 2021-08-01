using Bonebreaker.Inputs;
using Bonebreaker.Player;

namespace Bonebreaker.StateMachine
{
    public class IdleState : GroundedState
    {
        public IdleState (Character owner) : base(owner) {}

        protected override void _Enter (State previousState)
        {
            // Player idle animation
        }

        protected override void _CheckExitConditions (InputState input)
        {
            base._CheckExitConditions(input);

            if (Owner.GetInput().xInput != 0)
            {
                Owner.EnterState(Owner.RunningState);
            }
        }
    }
}