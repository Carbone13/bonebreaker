using System;
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

    public string GroundTag;
    
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
        AABB ground = new AABB(Collider.Position + new sfloat2(sfloat.Zero, Collider.HalfExtents.Y + (sfloat)0.2f), new sfloat2(Collider.HalfExtents.X, (sfloat)0.1f));

        foreach (AABB box in Physics.QueryBoxes())
        {
            if (box != Collider)
            {
                if (box.IntersectAABB(ground) != null)
                {
                   GroundTag = box.Tag;
                   return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Move this Body by the specified amount, it will stop as soon as it collide
    /// </summary>
    /// <param name="delta">The movement amounts</param>
    /// <param name="collided">Callback called on collision contact</param>
    public void MoveAndCollide (sfloat2 delta, Action<sfloat2> collided = null)
    {
        Sweep sweep = Collider.SweepInto(Physics.QueryBoxes(), delta, "Player");

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
            Sweep sweep = Collider.SweepInto(Physics.QueryBoxes(), delta, "Player");
            
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
                
                AddToPosition(traveled); 
            }
            // if we hit nothing, move to the target position and exit the loop
            else
            {
                sfloat2 traveled = sweep.Position - Collider.Position;
                totalTravel += traveled;
                
                AddToPosition(traveled);

                break;
            }
        }

        return totalTravel;
    }

    public void AddToPosition (sfloat2 a)
    {
        Position += a;
        Collider.Position += a;
        Hitbox.Position += a;
        Hurtbox.Position += a;
    }
}
