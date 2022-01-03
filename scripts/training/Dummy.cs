using Godot;

public class Dummy : Node2D
{
    private AnimationPlayer animator;
    
    public override void _Ready ()
    {
        GetNode<AABB>("Hurtbox").Connect(nameof(AABB.Ticked), this, nameof(Ticked));
        animator = GetNode<AnimationPlayer>("Animator");
    }

    public void Ticked (AABB dealer, int amount)
    {
        int dir = (int)sfloat.Sign(dealer.Position.X - (sfloat)GlobalPosition.x);
        
        if (dir == -1)
        {
            animator.Play("dummy_hit_left");
        }
        else
        {
            animator.Play("dummy_hit_right");
        }
        
        animator.Seek(0, true);
    }
}
