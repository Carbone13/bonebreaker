using Bonebreaker.Physics;
using FixedMath.NET;
using Godot;

public class Player : Actor2D
{
    public override void _PhysicsProcess (float delta)
    {
        FixedVector2 inputs = new FixedVector2()
        {
            X = (Fix64) Input.GetActionStrength("ui_right") - (Fix64) Input.GetActionStrength("ui_left"),
            Y = (Fix64) Input.GetActionStrength("ui_down") - (Fix64) Input.GetActionStrength("ui_up")
        };

        Move(inputs * (Fix64) 150 * (Fix64) delta, CollideX, CollideY);
    }

    public void CollideX (int sign)
    {
        
    }
    
    public void CollideY (int sign)
    {
        
    }
}
