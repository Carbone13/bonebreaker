using Godot;

namespace Bonebreaker.Player
{
    public class PlayerAnimator
    {
        public void Update (Character player)
        {
            AnimationPlayer animator = player.GetNode<AnimationPlayer>("Animator");
            string direction = "_" + (player.FacingDirection == 1 ? "right" : "left");
            string animation = "idle";
            string character = "";
            

            if (player.IsGrounded)
            {
                if (player.IsRunning)
                {
                    animation = "run";
                }
            }
            else
            {
                if (player.Velocity.Y > sfloat.Zero)
                {
                    animation = "ascend";
                }
                else
                {
                    animation = "fall";
                }
            }

            if (player.IsAttacking)
            {
                animation = "attack_" + player.AttackID;
            }

            if (character == "") return;
            
            animator.Play(character + animation + direction);
        }
    }
}