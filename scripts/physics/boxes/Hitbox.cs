using Godot;
using System;
using Bonebreaker.Physics;

[Tool]
public class Hitbox : Entity
{
    private Color color = new Color(0.741176f, 0.180392f, 0.352941f, 0.345098f);
    
    [Export] public bool Active = true;
    [Export] private int Width = 10;
    [Export] private int Height = 10;

    [Export(PropertyHint.Layers2dPhysics)] public int SearchOn = 1;

    public bool Tick ()
    {
        bool ticked = false;
        GD.Print("tick");
        foreach (Hurtbox hurtbox in Physic.GetHurtboxes())
        {
            GD.Print("against : " + hurtbox.Name);
            if (hurtbox.GetParent() == this.GetParent())
                continue;
            
            if ((hurtbox.FoundOn & this.SearchOn) == 0)
                continue;
            GD.Print("hey " + hurtbox.Name + " passed the test");
            if (Physic.IsColliding(Shape(), hurtbox.Shape()))
            {
                GD.Print("hit " + hurtbox.Name);
                ticked = true;
                hurtbox.EmitSignal(nameof(Hurtbox.Ticked), this);
            }
        }

        return ticked;
    }
    
    public Bonebreaker.Physics.AABB Shape () => new Bonebreaker.Physics.AABB (GlobalPosition, new int2(Width, Height));
    
    public override void _Process (float delta)
    {
        if (!Engine.EditorHint)
        {
            base._Process(delta);
        }
            
        Update();
    }

    public override void _Draw ()
    {
        if (!Active) return;
        
        DrawRect(new Rect2(Vector2.Zero, new Vector2(Width, Height)), color);
    }
}
