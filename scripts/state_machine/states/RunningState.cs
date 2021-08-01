using Bonebreaker.Inputs;
using Bonebreaker.Player;
using Godot;

namespace Bonebreaker.StateMachine
{
    public class RunningState : GroundedState
    {
        public RunningState (Character owner) : base(owner)
        {
        }

        protected override void _Tick (sfloat delta, uint frame, InputState input)
        {
            base._Tick(delta, frame, input);

            int dir = input.xInput;
            Owner.Velocity.X = (sfloat)dir * (sfloat)Owner.MOVE_SPEED;
        }

        protected override void _CheckExitConditions (InputState input)
        {
            base._CheckExitConditions(input);
            
            if (input.xInput == 0)
            {
                Owner.EnterState(Owner.IdleState);
            }
        }
    }
}