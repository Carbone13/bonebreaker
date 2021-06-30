using System.Collections.Generic;
using FixedMath.NET;
using Godot;

namespace Bonebreaker.Physics
{
    /// <summary>
    /// A Vector2 using Fixed Point
    /// </summary>
    public struct FixedVector2
    {
        public Fix64 X { get; set; }
        public Fix64 Y { get; set; }
        
        public FixedVector2 Normalize ()
        {
            Fix64 length = Length();

            Fix64 _X = X / length;
            Fix64 _Y = Y / length;

            return new FixedVector2(_X, _Y);
        }

        public Fix64 Length ()
        {
            return Fix64.Sqrt(X * X + Y * Y);
        }

        public static FixedVector2 operator *(FixedVector2 factorOne, Fix64 factorTwo)
        {
            return new FixedVector2(factorOne.X * factorTwo, factorOne.Y * factorTwo);
        }

        public override string ToString () => "x: " + X + "; y: " + Y;

        public FixedVector2 (Fix64 _x, Fix64 _y)
        {
            X = _x;
            Y = _y;
        }

        public FixedVector2 (float _x, float _y)
        {
            X = (Fix64) _x;
            Y = (Fix64) _y;
        }
        
        public FixedVector2 (string _x, string _y)
        {
            X = (Fix64) float.Parse(_x);
            Y = (Fix64) float.Parse(_y);
        }

        public string Serialize ()
        {
            return X + " - " + Y;
        }

        public static FixedVector2 Unserialize (string input)
        {
            return new FixedVector2(input.Trim().Split('-')[0].Trim(), input.Trim().Split('-')[1].Trim());
        }
    }

    public class CollisionHitInformations
    {
        public List<CollisionHit> Hits;

        public CollisionHitInformations ()
        {
            Hits = new List<CollisionHit>();
        }
    }

    public struct CollisionHit
    {
        public CollisionBox Box { get; set; }
        public Entity Entity { get; set; }
        
        public CollisionHit (CollisionBox _box, Entity _entity)
        {
            Box = _box;
            Entity = _entity;
        }
    }
}