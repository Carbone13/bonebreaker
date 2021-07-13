using System;
using System.Collections.Generic;
using Godot;

namespace Bonebreaker.Physics
{
    public class Actor : Entity
    {
        protected sfloat2 Remainder;

        protected Pushbox pushbox;

        public override void _Ready ()
        {
            base._Ready();
            foreach (Node child in GetChildren())
            {
                if (child is Pushbox _pushbox)
                {
                    if (pushbox == null)
                    {
                        pushbox = _pushbox;
                    }
                    else
                    {
                        GD.PrintErr("Multiple pushbox");
                    }
                }
            }
        }

        public bool Move (sfloat2 amount, int2 signedVelocity, Action<int> collideX = null, Action<int> collideY = null)
        {
            bool x = MoveHorizontaly(amount.X, signedVelocity, collideX);
            bool y = MoveVerticaly(amount.Y, signedVelocity, collideY);

            return x || y;
        }

        public bool MoveHorizontaly (sfloat amount, int2 signedVelocity, Action<int> collide = null)
        {
            Remainder.X += amount;
            int pixelToMove = (int) libm.roundf(Remainder.X);

            if (pixelToMove != 0)
            {
                Remainder.X -= (sfloat) pixelToMove;
                int sign = Math.Sign(pixelToMove);

                while (pixelToMove != 0)
                {
                    if (!Physic.CollideAt(pushbox, pushbox.Position + new int2(sign, 0), signedVelocity))
                    {
                        Position += new int2(sign, 0);
                        
                        foreach (Node child in GetChildren())
                        {
                            if (child is Entity entity)
                            {
                                entity.Position += new int2(sign, 0);
                            }
                        }
                        pixelToMove -= sign;
                    }
                    else
                    {
                        Remainder.X = sfloat.Zero;
                        collide?.Invoke(sign);
                        break;
                    }
                }

                return true;
            }

            return false;
        }

        public bool MoveVerticaly (sfloat amount, int2 signedVelocity, Action<int> collide = null)
        {
            Remainder.Y += amount;
            int pixelToMove = (int) libm.roundf(Remainder.Y);

            if (pixelToMove != 0)
            {
                Remainder.Y -= (sfloat) pixelToMove;
                int sign = Math.Sign(pixelToMove);

                while (pixelToMove != 0)
                {
                    if (!Physic.CollideAt(pushbox, pushbox.Position + new int2(0, sign), signedVelocity))
                    {
                        Position += new int2(0, sign);
                        
                        foreach (Node child in GetChildren())
                        {
                            if (child is Entity entity)
                            {
                                entity.Position += new int2(0, sign);
                            }
                        }
                        pixelToMove -= sign;
                    }
                    else
                    {
                        Remainder.Y = sfloat.Zero;
                        collide?.Invoke(sign);
                        break;
                    }
                }

                return true;
            }

            return false;
        }
    }
}