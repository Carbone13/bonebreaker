using Godot;
using System;
using System.Linq;
using FixedMath.NET;

namespace Bonebreaker.Physics
{
    public abstract class Actor2D : Entity, ICollidable
    {
        // Editor
        [Export] private NodePath pushbox;
        
        public CollisionBox Pushbox { get; private set; }

        private FixedVector2 remainder;

        public override void _Ready ()
        {
            Pushbox = GetNode<CollisionBox>(pushbox);
            Pushbox.Type = CollisionBoxType.Pushbox;
            Pushbox.OwningEntity = this;
            Area2D t = new Area2D();
            
        }
        
        protected bool Move (FixedVector2 amount, Action<int> collideX, Action<int> collideY)
        {
            bool x = MoveX(amount.X, Fix64.Sign(amount.Y), collideX);
            bool y = MoveY(amount.Y, collideY);

            return x || y;
        }

        private bool MoveX (Fix64 amount, int yDir, Action<int> collideX)
        {
            bool moved = false;
            
            remainder.X += amount;
            int move = (int) Fix64.Round(remainder.X);

            if (move != 0)
            {
                int sign = Math.Sign(move);

                while (move != 0)
                {
                    CollisionHitInformations hits = Pushbox.CollideAtPosition(GlobalPosition + new Vector2(sign, 0));

                    if (hits.Hits.Count == 0)
                    {
                        move -= sign;
                        remainder.X -= (Fix64) sign;
                        RealPosition = new FixedVector2() {X = RealPosition.X + (Fix64) sign, Y = RealPosition.Y};

                        moved = true;
                    }
                    else
                    {
                        CollisionHitInformations previousHits = Pushbox.CollideAtPosition(GlobalPosition);
                        if (previousHits.Hits.Count > 0)
                        {
                            move -= sign;
                            remainder.X -= (Fix64)sign;
                            RealPosition = new FixedVector2() { X = RealPosition.X + (Fix64)sign, Y = RealPosition.Y };

                            moved = true;
                        }
                        else
                        {
                            remainder.X = Fix64.Zero;
                            collideX?.Invoke(sign);
                            break;
                        }
                    }
                }
            }

            return moved;
        }

        private bool MoveY (Fix64 amount, Action<int> collideY)
        {
            bool moved = false;
            
            remainder.Y += amount;
            int move = (int) Fix64.Round(remainder.Y);

            if (move != 0)
            {
                int sign = Math.Sign(move);

                while (move != 0)
                {
                    CollisionHitInformations hits = Pushbox.CollideAtPosition(GlobalPosition + new Vector2(0, sign));

                    if (hits.Hits.Count == 0)
                    {
                        move -= sign;
                        remainder.Y -= (Fix64)sign;
                        RealPosition = new FixedVector2() { X = RealPosition.X, Y = RealPosition.Y + (Fix64)sign };

                        moved = true;
                    }
                    else
                    {
                        CollisionHitInformations previousHits = Pushbox.CollideAtPosition(GlobalPosition);
                        if (previousHits.Hits.Count > 0)
                        {
                            move -= sign;
                            remainder.Y -= (Fix64)sign;
                            RealPosition = new FixedVector2() { X = RealPosition.X, Y = RealPosition.Y + (Fix64)sign };

                            moved = true;
                        }
                        else
                        {
                            remainder.Y = Fix64.Zero;
                            collideY?.Invoke(sign);
                            break;
                        }
                    }
                }
            }

            return moved;
        }
    }
}