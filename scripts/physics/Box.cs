using Godot;
using Godot.Collections;

[Tool]
public class Box : Node2D
{
    public Vector2 Size;

    private bool Draw
    {
        get => _draw;
        set
        {
            _draw = value;
            PropertyListChangedNotify();
        }
        
    }
    private Color Color = Colors.Aqua;

    private bool _draw;

    public override Array _GetPropertyList ()
    {
        var properties = new Array();
        
        properties.Add(new Dictionary 
        { 
            {"name", nameof(Size)}, 
            {"type", Variant.Type.Vector2},
            {"usage", PropertyUsageFlags.Default} 
        });
        properties.Add(new Dictionary 
        { 
            {"name", nameof(Draw)}, 
            {"type", Variant.Type.Bool},
            {"usage", PropertyUsageFlags.Default} 
        });

        if (_draw)
        {
            properties.Add(new Dictionary 
            { 
                {"name", nameof(Color)}, 
                {"type", Variant.Type.Color},
                {"usage", PropertyUsageFlags.Default} 
            });
        }

        return properties;
    }

    public override void _PhysicsProcess (float delta)
    {
        Update();
    }

    public override void _Draw ()
    {
        if(Draw)
            DrawRect(new Rect2(Vector2.Zero - Size / 2, Size), Color);
    }
}
