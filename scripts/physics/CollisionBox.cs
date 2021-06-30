using Godot;
using System;
using Bonebreaker.GameManagement;

namespace Bonebreaker.Physics
{
    public enum CollisionBoxType
    {
        Pushbox,
        Hitbox,
        Hurtbox
    }
    [Tool]
    public class CollisionBox : Node2D
    {
        [Export] public string Tag;
        [Export] private Color PUSHBOX_COLOR = Colors.LightSkyBlue;
        [Export] private Color HITBOX_COLOR = Colors.Firebrick;
        [Export] private Color HURTBOX_COLOR = Colors.Chartreuse;
        
        [Export] private Rect2 Settings;
        [Export] private bool Draw;
        
        public Entity OwningEntity { get; set; }
        public CollisionBoxType Type { get; set; }
        public Rect2 Bounds (Vector2 position) => new Rect2(position + Settings.Position, Scale * Settings.Size);
        
        public override void _ExitTree ()
        {
            GameManager.singleton.Tracker.Unsubscribe(this);
        }

        public override void _EnterTree ()
        {
            GameManager.singleton.Tracker.Subscribe(this);
        }

        public override void _Process (float delta)
        {
            Update();
        }

        public override void _Draw ()
        {
            if (Draw)
            {
                DrawRect(Bounds(Vector2.Zero), PUSHBOX_COLOR);
            }
        }
        
        public CollisionHitInformations CollideAtPosition (Vector2 position)
        {
            CollisionHitInformations informations = new CollisionHitInformations();
            
            foreach (CollisionBox box in GameManager.singleton.Tracker.GetBoxes(CollisionBoxType.Pushbox))
            {
                if (box != this && box.Bounds(box.GlobalPosition).Intersects(this.Bounds(position)))
                {
                    informations.Hits.Add(new CollisionHit(box, box.OwningEntity));
                }
            }

            return informations;
        }
    }
}