using Godot.Collections;

namespace Bonebreaker.Network
{
    public interface IRollbackable
    {
        public Dictionary<string, string> SaveState ();
        public void LoadState (Dictionary<string, string> state);
    }
}