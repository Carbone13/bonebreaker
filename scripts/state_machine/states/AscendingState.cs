using Bonebreaker.Inputs;
using Bonebreaker.Player;

namespace Bonebreaker.StateMachine
{
    public class AscendingState : InAirState
    {
        public AscendingState (Character owner) : base(owner)
        {
        }
        
        protected override void _CheckExitConditions (InputState input)
        {
            base._CheckExitConditions(input);
            
            if(Owner.Velocity.Y > sfloat.Zero)
                Owner.EnterState(Owner.FallingState);
        }
    }
}