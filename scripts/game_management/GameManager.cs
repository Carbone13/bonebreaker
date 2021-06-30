using Godot;
using System;
using Bonebreaker.Physics;

namespace Bonebreaker.GameManagement
{
    public class GameManagerModule
    {
        public virtual void Initialize () {}
    }
    
    public class GameManager : Node
    {
        public static GameManager singleton;
        
        public PhysicTracker Tracker;
        
        public override void _Ready ()
        {
            Tracker = new PhysicTracker();
            Tracker.Initialize();

            singleton = this;
        }
    }
}