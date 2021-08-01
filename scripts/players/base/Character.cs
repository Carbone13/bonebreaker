using Bonebreaker.Inputs;
using Bonebreaker.Physics;
using Bonebreaker.StateMachine;
using Godot;
using Input = Bonebreaker.Inputs.Input;

namespace Bonebreaker.Player
{
    public abstract class Character : Actor
    {
        [Export] public int MOVE_SPEED = 75;
        [Export] public int JUMP_HEIGHT = 50;
        [Export] public float JUMP_APEX_TIME = 0.33f;
        [Export] public int COYOTE_TIME_MS = 70;
        [Export] public int DEVICE_ID = -1;
        
        public sfloat Gravity;
        public sfloat JumpForce;

        public bool IsGrounded;
        public sfloat2 Velocity;

        private uint _frame;

        public IdleState IdleState;
        public RunningState RunningState;
        public FallingState FallingState;
        public AscendingState AscendingState;

        public State CurrentState;
        
        /// <summary>
        /// Get the Input of this Player, according to DEVICE_ID
        /// </summary>
        public InputState GetInput () => Input.singleton.GetStateFromID(DEVICE_ID);

        public override void _Ready ()
        {
            base._Ready();
            
            InitializeStateMachine();
            CalculatePhysicsConstants();
            CheckIfGrounded();
        }

        /// <summary>
        /// Do some basic calculus in order to resolve the velocity and jump force
        /// </summary>
        private void CalculatePhysicsConstants ()
        {
            Gravity = (sfloat)JUMP_HEIGHT * (sfloat)2 / ((sfloat)JUMP_APEX_TIME * (sfloat) (JUMP_APEX_TIME));
            JumpForce = libm.sqrtf((sfloat)2 * Gravity * (sfloat)JUMP_HEIGHT);
        }

        /// <summary>
        /// Initialize the State Machine with basic states
        /// Characters override it to provides their own custom states (mostly for attacks)
        /// </summary>
        protected virtual void InitializeStateMachine ()
        {
            IdleState = new IdleState(this);
            RunningState = new RunningState(this);
            FallingState = new FallingState(this);
            AscendingState = new AscendingState(this);

            EnterState(IdleState);
        }
        
        /// <summary>
        /// Enter a specified State, recursively enter state until it stabilize in one state
        /// </summary>
        /// <param name="state">The State instance you want to enter</param>
        public void EnterState (State state)
        {
            // Exit Current State
            CurrentState?.Exit(state);
            // Set the new state as the Current State
            CurrentState = state;
            // Enter Current State
            CurrentState.Enter();
            
            ResolveCorrectState();
        }
        
        /// <summary>
        /// Check the exit conditions of newly entered state, until it do not result on a state change
        /// </summary>
        public void ResolveCorrectState ()
        {
            State originalState = CurrentState;
            // Check the exit condition
            originalState.CheckExitConditions(GetInput());
            // Actually do it until it don't change our state
            while (originalState != CurrentState)
            {
                originalState = CurrentState;
                CurrentState.CheckExitConditions(GetInput());
            }
        }
        
        /// <summary>
        /// Check if this Character is currently grounded
        /// </summary>
        public void CheckIfGrounded ()
        {
            // Check if we are not going up && if their is something directly below us
            IsGrounded = Velocity.Y >= sfloat.Zero && 
                Physic.CollideAt(pushbox, pushbox.Position + new int2(0, 1), new int2());
        }
        
        // TODO Debug Only
        // Simulating the SyncManager
        public override void _PhysicsProcess (float delta)
        {
            Tick((sfloat)delta, _frame, GetInput());
            _frame++;
        }

        public void Tick (sfloat delta, uint frame, InputState input)
        {
            CurrentState.Tick(delta, frame, input);
            //Move(new sfloat2(50, 50) * delta, new int2(1, 0), null, null);
        }


        /// <summary>
        /// Move the Character, check if he become grounded or not, and switch the states accordingly
        /// </summary>
        /// <param name="delta">The delta time</param>
        public void ProceedMovement (sfloat delta)
        {
            bool hasMoved = Move(Velocity * delta, new int2(Velocity), CollidedX, CollidedY);

            // If we moved
            if (hasMoved)
            {
                // Check if we are now grounded, or if we stopped being grounded
                CheckIfGrounded();
                // And resolve the new correct state
                ResolveCorrectState();
            }
        }
        
        /// <summary>
        /// Collided on the X Axis, Reset the velocity and resolve the new correct state
        /// </summary>
        /// <param name="direction">The direction we were going</param>
        private void CollidedX (int direction)
        {
            Velocity.X = sfloat.Zero;
            ResolveCorrectState();
        }

        /// <summary>
        /// Collided on the Y Axis, Reset the velocity and resolve the new correct state, if going down, set IsGrounded to true
        /// </summary>
        /// <param name="direction"></param>
        private void CollidedY (int direction)
        {
            Velocity.Y = sfloat.Zero;

            if (direction == 1)
                IsGrounded = true;
            
            ResolveCorrectState();
        }
    }
}