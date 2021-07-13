using Godot;

namespace Bonebreaker.Physics
{
    [Tool]
    public class Entity : Node2D
    {
        private int2 _position;
        public new int2 Position
        {
            get => _position;
            set
            {
                int2 old = _position;
                _position = value;
                Moved(old);
            }
        }

        public override void _Ready ()
        {
            Position = new int2(GlobalPosition.x, GlobalPosition.y);
        }

        public override void _Process (float delta)
        {
            if (Engine.EditorHint)
                Position = new int2(GlobalPosition.x, GlobalPosition.y);
        }

        protected virtual void Moved (int2 oldPosition)
        {
            GlobalPosition = Position;
        }
    }
}