using Godot;
using System;
using Bonebreaker.Physics;

public class Dummy : Node2D
{
    private Hurtbox hurtbox;
    private AnimationPlayer animator;
    
    public override void _Ready ()
    {
        hurtbox = GetNode<Hurtbox>("Hurtbox");
        animator = GetNode<AnimationPlayer>("Animator");
        hurtbox.Connect(nameof(Hurtbox.Ticked), this, nameof(Ticked));
    }

    public void Ticked (Hitbox ticker)
    {
        int direction = Math.Sign(ticker.Position.X - GlobalPosition.x);
        GD.Print(direction);
        if (direction < 0)
        {
            animator.Play("dummy_hit_left");
        }
        else
        {
            animator.Play("dummy_hit_right");
        }
    }
}
