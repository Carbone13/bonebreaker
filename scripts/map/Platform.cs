using Godot;

[Tool]
public class Platform : Node
{
    private int _width;
    private bool _rightPart, _leftPart;
    
    private Node2D edgeA, edgeB;
    private Sprite middlePart;
    private AABB hitbox;

    [Export]
    private bool rightPart
    {
        get => _rightPart;
        set
        {
            if (!Engine.EditorHint) return;
            _rightPart = value;
            UpdateWidth();
        }
    }

    [Export]
    private bool leftPart
    {
        get => _leftPart;
        set
        {
            if (!Engine.EditorHint) return;
            _leftPart = value;
            UpdateWidth();
        }
    }
    [Export(PropertyHint.Range, "10, 320, 2")]
    private int width
    {
        get => _width;
        set
        {
            if (!Engine.EditorHint) return;
            _width = value;
            UpdateWidth();
        }
    }

    public override void _Ready ()
    {
        if (Engine.EditorHint)
        {
            edgeA = GetNode<Node2D>("Visual/edge_a");
            edgeB = GetNode<Node2D>("Visual/edge_b");
            middlePart = GetNode<Sprite>("Visual/middle_part");
            hitbox = GetNode<AABB>("Hitbox");
        }
    }

    private void UpdateWidth ()
    {
        if (!Engine.EditorHint) return;
        
        edgeB.Visible = rightPart;
        edgeA.Visible = leftPart;

        
        var middlePartSize = width - 52 + (rightPart ? 0 : 26) + (leftPart ? 0 : 26);

        var leftPartPosition = -13 - middlePartSize / 2;
        var rightPartPosition = 13 + middlePartSize / 2;

        edgeA.Position = new Vector2(leftPartPosition, edgeA.Position.y);
        edgeB.Position = new Vector2(rightPartPosition, edgeB.Position.y);

        middlePart.RegionRect = new Rect2(0, 0, middlePartSize, 10);

        hitbox.Size = new Vector2(width / 2, 5);
    }
    
}
