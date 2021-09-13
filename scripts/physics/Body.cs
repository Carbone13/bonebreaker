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
    [Export] public NodePath Pushbox_path, Hitbox_path, Hurtbox_path;

    protected AABB Collider;
    protected AABB Hurtbox;
    protected AABB Hitbox;
    
    public override void _Ready ()
    {
        base._Ready();
        Collider = GetNode<AABB>(Pushbox_path);
        
        if (Hurtbox_path != null)
            Hurtbox = GetNode<AABB>(Hurtbox_path);
        if (Hitbox_path != null)
            Hitbox = GetNode<AABB>(Hitbox_path);
    }

    /// <summary>
    /// Check if their is a collider directly below us
    /// </summary>
    /// <returns></returns>
    protected bool IsGrounded ()
    {
        return Physics.CastAABB(Collider.Position + new sfloat2(0, 0.2f), Collider.HalfExtents - new sfloat2(0.1f, 0.1f),
            true, new List<Predicate<AABB>> { (box => box != Collider), (aabb => aabb.Type == (int)Boxes.Pushbox), (aabb => aabb.IntersectAABB(Collider) == null) }).Count > 0;
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
        
        sfloat2 traveled = sweep.Position - Collider.Position;

        Collider.Position += traveled;
        Hitbox.Position += traveled;
        Hurtbox.Position += traveled;
    }
    
    /// <summary>
    /// Move this Body by the specified amount, if it collide, it will continue to consume
    /// its velocity on the other axis
    /// </summary>
    /// <param name="delta">The movement amounts</param>
    /// <param name="collided">Callback called on collision contact</param>
    protected sfloat2 MoveAndSlide (sfloat2 delta, Action<sfloat2> collided = null)
    {
        sfloat2 totalTravel = sfloat2.Zero;
        
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
                sfloat2 traveled = sweep.Position - Collider.Position;
                totalTravel += traveled;
                
                Position += traveled;
                Collider.Position += traveled;
                Hitbox.Position += traveled;
                Hurtbox.Position += traveled;
            }
            // if we hit nothing, move to the target position and exit the loop
            else
            {
                sfloat2 traveled = sweep.Position - Collider.Position;
                totalTravel += traveled;
                
                Position += traveled;
                Collider.Position += traveled;
                Hitbox.Position += traveled;
                Hurtbox.Position += traveled;

                break;
            }
        }

        return totalTravel;
    }
    
}
