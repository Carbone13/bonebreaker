using Godot;

namespace Bonebreaker.Player
{
    public class Marston : Character
    {
        [Export] public AudioStream Punch_SFX;
        private uint lastAttackTime;
        
        public void AttackFinished ()
        {
            IsAttacking = false;
        }
        
        public override void Light (sfloat delta, uint frame)
        {
            if ((sfloat)(frame - lastAttackTime) > (sfloat)31)
            {
                AttackID = 0;
            }
            
            if (AttackID == 0 || AttackID == 3)
            {
                if (!IsAttacking && (sfloat)(frame - lastAttackTime) > (sfloat)31)
                {
                    IsAttacking = true;
                
                    AttackID++;
                    if (AttackID > 3) AttackID = 1;

                    lastAttackTime = frame;

                    if (GetNode<Hitbox>("Hitbox").Tick())
                    {
                        GetNode<AudioStreamPlayer2D>("Audio").Stream = Punch_SFX;
                        GetNode<AudioStreamPlayer2D>("Audio").Play();
                    }
                        
                }
            }
            else
            {
                if (!IsAttacking && (sfloat)(frame - lastAttackTime) > (sfloat)13)
                {
                    IsAttacking = true;
                
                    AttackID++;

                    lastAttackTime = frame;
                    if (GetNode<Hitbox>("Hitbox").Tick())
                    {
                        GetNode<AudioStreamPlayer2D>("Audio").Stream = Punch_SFX;
                        GetNode<AudioStreamPlayer2D>("Audio").Play();
                    }
                        
                }
            }

            if(IsAttacking) Velocity = sfloat2.Zero;
        }

        public override void Special ()
        {
            
        }
    }
}