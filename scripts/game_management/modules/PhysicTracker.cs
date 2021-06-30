using Godot;
using System;
using System.Collections.Generic;
using Bonebreaker.GameManagement;

namespace Bonebreaker.Physics
{
    public class PhysicTracker : GameManagerModule
    {
        public Dictionary<CollisionBoxType, List<CollisionBox>> Boxes;

        public override void Initialize ()
        {
            Boxes = new Dictionary<CollisionBoxType, List<CollisionBox>>();
            
            Boxes[CollisionBoxType.Pushbox] = new List<CollisionBox>();
            Boxes[CollisionBoxType.Hitbox] = new List<CollisionBox>();
            Boxes[CollisionBoxType.Hurtbox] = new List<CollisionBox>();
        }

        public void Subscribe (CollisionBox box)
        {
            Boxes[box.Type].Add(box);
        }
        
        public void Unsubscribe (CollisionBox box)
        {
            Boxes[box.Type].Remove(box);
        }

        public List<CollisionBox> GetBoxes (CollisionBoxType ofType)
        {
            return Boxes[ofType];
        }
    }
}

