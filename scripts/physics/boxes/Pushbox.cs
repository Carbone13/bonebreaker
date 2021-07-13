using Bonebreaker.Physics.Broadphase;
using System.Collections.Generic;
using Godot;
using Godot.Collections;

namespace Bonebreaker.Physics
{
    [Tool]
    public class Pushbox : Entity, IGridElement
    {
        #region Editor
        [Export] private Color color = new Color(0.094118f, 0.462745f, 0.52549f, 0.345098f);
        [Export] public bool Active = true;
        [Export] private int Width = 10;
        [Export] private int Height = 10;
        [Export(PropertyHint.Layers2dPhysics)] public int FoundOn = 1;
        [Export(PropertyHint.Layers2dPhysics)] public int SearchOn = 1;
        [Export] public Array<Vector2> IgnoredDirections;

        public override void _Ready ()
        {
            if(!Engine.EditorHint)
                Physic.Register(this);
            base._Ready();
        }

        protected override void Moved (int2 oldPosition)
        {
            base.Moved(oldPosition);
            if(!Engine.EditorHint)
                Physic.Update(this, oldPosition);
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

        #endregion

        #region Public Fields

        public AABB Shape () => new AABB(Position, new int2(Width, Height));
        public AABB Shape (int2 position) => new AABB(position, new int2(Width, Height));

        #endregion

        public System.Collections.Generic.Dictionary<int2, IGridElement> NextElement { get; set; } = new System.Collections.Generic.Dictionary<int2, IGridElement>();
        public int2 Min => Shape().Min;
        public int2 Max => Shape().Max;
        public int ID { get; set; }
    }
}