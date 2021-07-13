using Bonebreaker.Inputs;
using Bonebreaker.Physics;
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

        private PlayerAnimator Animator;
        
        private sfloat _gravity;
        private sfloat _jumpForce;

        private uint _lastGroundedTime;

        public int FacingDirection = 1;
        public bool IsGrounded;
        public bool IsJumping;
        public bool IsRunning;
        public bool IsFalling;
        public bool IsStunned;
        public bool IsAttacking;
        public int AttackID;
        
        public bool CanMove;
        public bool CanJump;

        public sfloat2 Velocity;

        private uint _frame;

        public override void _Ready ()
        {
            Animator = new PlayerAnimator();
            
            base._Ready();
            
            _gravity = (sfloat)JUMP_HEIGHT * (sfloat)2 / ((sfloat)JUMP_APEX_TIME * (sfloat) (JUMP_APEX_TIME));
            _jumpForce = libm.sqrtf((sfloat)2 * _gravity * (sfloat)JUMP_HEIGHT);
        }

        public InputState GetInput ()
        {
            return Input.singleton.GetStateFromID(DEVICE_ID);
        }

        public override void _PhysicsProcess (float delta)
        {
            Tick((sfloat)delta, _frame, GetInput());
            _frame++;
        }

        public void Tick (sfloat delta, uint frame, InputState input)
        {
            Velocity.X = (sfloat) input.xInput * (sfloat) MOVE_SPEED;

            if (input.Jump && !IsJumping && (IsGrounded || (sfloat)(frame - _lastGroundedTime) < (sfloat)COYOTE_TIME_MS))
            {
                Velocity.Y = -_jumpForce;
                IsJumping = true;
                IsGrounded = false;
            }
            else if (!IsGrounded)
            {
                if (input.Fall)
                {
                    Velocity.Y += _gravity * (sfloat)1.4f * delta;
                }
                else
                {
                    Velocity.Y += _gravity * delta;
                }
            }
            
            // We call the action function just before moving, so they can alter the velocity
            if(input.Light)
                Light(delta, frame);
            
            bool hasMoved = Move(Velocity * delta, new int2(Velocity), CollidedX, CollidedY);

            if (hasMoved)
            {
                if (Remainder.Y == sfloat.Zero)
                {
                    IsGrounded = Physic.CollideAt(pushbox, pushbox.Position + new int2(0, 1), new int2());
                }
            }

            if (IsGrounded)
            {
                _lastGroundedTime = frame;
            }

            IsRunning = Velocity.X != sfloat.Zero;
            
            if(Velocity.X.Sign() != 0)
                FacingDirection = Velocity.X.Sign();
            
            Animator.Update(this);
        }

        private void CollidedX (int direction)
        {
            Velocity.X = sfloat.Zero;
        }

        private void CollidedY (int direction)
        {
            Velocity.Y = sfloat.Zero;

            if (direction == 1)
            {
                IsGrounded = true;
                IsJumping = false;
            }
                
        }

        public abstract void Light (sfloat delta, uint frame);
        public abstract void Special ();
    }
}