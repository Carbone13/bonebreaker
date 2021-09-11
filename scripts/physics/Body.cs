using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

/// <summary>
/// An entity able to move in the scene while colliding with its surrounding
/// Need an attached Collider
/// </summary>
public class Body : Entity
{
    [Export] public NodePath Pushbox, Hitbox, Hurtbox;

    protected AABB Collider;

    public override void _Ready ()
    {
        base._Ready();
        Collider = GetNode<AABB>(Pushbox);
    }

    /// <summary>
    /// Check if their is a collider directly below us
    /// </summary>
    /// <returns></returns>
    protected bool IsGrounded ()
    {
        return Physics.CastAABB(Collider.Position + new sfloat2(0, 0.2f), Collider.HalfExtents - new sfloat2(0.1f, 0.1f),
            true, new List<AABB> { Collider }).Count > 0;
    }

    /// <summary>
    /// Move this Body by the specified amount, it will stop as soon as it collide
    /// </summary>
    /// <param name="delta">The movement amounts</param>
    /// <param name="collided">Callback called on collision contact</param>
    public void MoveAndCollide (sfloat2 delta, Action<sfloat2> collided = null)
    {
        Sweep sweep = Collider.SweepInto(Physics.QueryBoxes(), delta);

        if (sweep.Hit != null)
        {
            collided?.Invoke(sweep.Hit.Normal);
        }
        
        Position = sweep.Position;
        Collider.Position = Position;
    }
    
    /// <summary>
    /// Move this Body by the specified amount, if it collide, it will continue to consume
    /// its velocity on the other axis
    /// </summary>
    /// <param name="delta">The movement amounts</param>
    /// <param name="collided">Callback called on collision contact</param>
    protected void MoveAndSlide (sfloat2 delta, Action<sfloat2> collided = null)
    {
        // While we still have movement to consume
        while (delta != sfloat2.Zero)
        {
            // Try to move by this movement
            Sweep sweep = Collider.SweepInto(Physics.QueryBoxes(), delta);

            // hit something
            if (sweep.Hit != null)
            {
                collided?.Invoke(sweep.Hit.Normal);
                // remove the traveled distance from the buffer
                delta -= sweep.Position - Collider.Position;

                // Depending on which axis we did collide, put the buffer to 0
                // because as we collided we won't be able to move any further
                if (sweep.Hit.Normal.X != sfloat.Zero)
                    delta.X = sfloat.Zero;
                if (sweep.Hit.Normal.Y != sfloat.Zero)
                    delta.Y = sfloat.Zero;
                
                // Move to the furthest possible position
                // And re-iterate one more time if we still have movement to consume
                sfloat2 colliderOffset = Collider.Position - Position;
                Position = sweep.Position - colliderOffset;
                Collider.Position = sweep.Position;
            }
            // if we hit nothing, move to the target position and exit the loop
            else
            {
                sfloat2 colliderOffset = Collider.Position - Position;
                Position = sweep.Position - colliderOffset;
                Collider.Position = sweep.Position;

                break;
            }
        }
    }
    
}
