using Godot;

namespace Bonebreaker.Network
{
    public class SyncManagerWrapper : Node
    {
        private static SyncManagerWrapper _instance;
        public Node _SM;
        
        public override void _EnterTree ()
        {
            _instance = this;
            _SM = GetTree().Root.GetNode("SyncManager");
        }

        public static sfloat GetDeltaTime () => (sfloat)(float)_instance._SM.Get("tick_time");
        public static int GetCurrentTick () => (int)_instance._SM.Get("current_tick");
    }
}