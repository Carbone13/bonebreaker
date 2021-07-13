using Godot;

namespace Bonebreaker.Physics
{
    [Tool]
    public class Hurtbox : Entity
    {
        private Color color = new Color(0.180392f, 0.741176f, 0.189154f, 0.345098f);
    
        [Export] public bool Active = true;
        [Export] private int Width = 10;
        [Export] private int Height = 10;
        [Export(PropertyHint.Layers2dPhysics)] public int FoundOn = 1;

        [Signal]
        public delegate void Ticked (Hitbox ticker);
        
        public AABB Shape () => new AABB(GlobalPosition, new int2(Width, Height));


        public override void _Ready ()
        {
            base._Ready();
            if (!Engine.EditorHint)
                Physic.Register(this);
        }

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
}