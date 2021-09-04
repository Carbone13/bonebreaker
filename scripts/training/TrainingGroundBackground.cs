using Godot;
using Vector2 = Godot.Vector2;

[Tool]
public class TrainingGroundBackground : Node2D
{
    public override void _Draw ()
    {
        Color innerGrid = new Color(0.784314f, 0.788235f, 0.796078f);
        Color outerGrid = new Color(0.705882f, 0.705882f, 0.705882f);

        for (int i = -20; i < 320; i+= 4)
        {
            if (i < 0) continue;
            DrawLine(new Godot.Vector2(i, 0), new Godot.Vector2(i, 180), innerGrid);
        }
        for (int i = -20; i < 180; i+= 4)
        {
            if (i < 0) continue;
            DrawLine(new Godot.Vector2(0, i), new Godot.Vector2(320, i), innerGrid);
        }
        for (int i = -20; i < 320; i+= 20)
        {
            if (i < 0) continue;
            DrawLine(new Godot.Vector2(i, 0), new Godot.Vector2(i, 180), outerGrid, 1.0000f, true);
        }
        for (int i = -20; i < 180; i += 20)
        {
            if (i < 0) continue;
            DrawLine(new Godot.Vector2(0, i), new Godot.Vector2(320, i), outerGrid, 1.0000f, true);
        }
        
        
        DrawLine(new Godot.Vector2(40, 0), new Godot.Vector2(40, 180), Colors.Blue, 1.0000f, true);
        DrawLine(new Godot.Vector2(40 + 40 * 6, 0), new Godot.Vector2(40 + 40 * 6, 180), Colors.Blue, 1, true);
        DrawLine(new Godot.Vector2(0, 40), new Godot.Vector2(320, 40), Colors.Blue, 1.0000f, true);
        
        DrawLine(new Godot.Vector2(160, 0), new Godot.Vector2(160, 180), Colors.Red, 1.0000f, true);
        DrawLine(new Godot.Vector2(0, 140), new Godot.Vector2(320, 140), Colors.Red, 1.0000f, true);
    }
}
