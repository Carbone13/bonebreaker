using System;
using System.Collections.Generic;
using Bonebreaker.Physics.Broadphase;
using Godot;

namespace Bonebreaker.Physics
{
    public class Physic : Node2D
    {
        private static Physic singleton;

        private GridDrawer _drawer;

        private SpatialHashGrid<Pushbox> pushboxGrid;
        private List<Hurtbox> _hurtboxes = new List<Hurtbox>();
        
        
        public override void _Ready ()
        {
            GridDrawer drawer = new GridDrawer();
            AddChild(drawer);

            _drawer = drawer;
    
            pushboxGrid = new SpatialHashGrid<Pushbox>(20, 20, 1200, 1200);
            //_drawer.target = pushboxGrid;
            singleton = this;
        }

        public static List<Hurtbox> GetHurtboxes ()
        {
            return singleton._hurtboxes;
        }
        
        public static void Register (Pushbox pushbox)
        {
            singleton.pushboxGrid.Insert(pushbox);
        }

        public static void Register (Hurtbox hurtbox)
        {
            singleton._hurtboxes.Add(hurtbox);
        }

        public static void Update (Pushbox pushbox, int2 oldPosition)
        {
            int2 oldMin = oldPosition;
            int2 oldMax = oldPosition + (pushbox.Max - pushbox.Min);
            
            singleton.pushboxGrid.Move(pushbox, oldMin, oldMax);
        }

        public static bool IsColliding (AABB a, AABB b)
        {
            if (a.Max.X <= b.Min.X || a.Min.X >= b.Max.X) return false;
            if (a.Max.Y <= b.Min.Y || a.Min.Y >= b.Max.Y) return false;

            return true;
        }

        public static IEnumerable<Pushbox> BroadphaseQuery<T> (AABB shape) where T : Pushbox
        {
            return singleton.pushboxGrid.Query(shape);
        }

        public static bool CollideAt (Pushbox box, int2 Position)
        {
            if (!box.Active) return false;

            foreach (Pushbox queriedBox in BroadphaseQuery<Pushbox>(box.Shape(Position)))
            {
                if (!queriedBox.Active) continue;
                if ((box.SearchOn & queriedBox.FoundOn) == 0) continue;
                if (box == queriedBox) continue;
                
                if (IsColliding(box.Shape(Position), queriedBox.Shape()))
                {
                    return true;
                }
            }
            
            return false;
        }
        
        public static bool CollideAt (Pushbox box, int2 Position, int2 velocity)
        {
            if (!box.Active) return false;

            foreach (Pushbox queriedBox in BroadphaseQuery<Pushbox>(box.Shape(Position)))
            {
                if (!queriedBox.Active) continue;
                if ((box.SearchOn & queriedBox.FoundOn) == 0) continue;
                if (box == queriedBox) continue;
                if (queriedBox.IgnoredDirections.Contains(velocity)) continue;
                    
                if (IsColliding(box.Shape(Position), queriedBox.Shape()))
                {
                    if(IsColliding(box.Shape(), queriedBox.Shape()))
                            continue;
                    
                    return true;
                }
            }
            
            return false;
        }

    }
}