using Bonebreaker.Inputs;
using Godot;
using Godot.Collections;
using Input = Bonebreaker.Inputs.Input;

namespace Bonebreaker.scripts.player
{
    public class DebugPlayer : Body
    {
        public sfloat2 Velocity { get; set; }
        public Dictionary i;
        
        public void _network_spawn (Dictionary data)
        {
            SetNetworkMaster((int)data["peer_id"]);
            Name = "Player " + (int)data["peer_id"];
        }
        
        public Dictionary _get_local_input ()
        {
            return Input.singleton.GetStateOfPrimaryDevice().Serialize();
        }
        
        private void _network_process (Dictionary input)
        {
            InputState inp = InputState.Deserialize(input);
            i = input;
            Velocity = new sfloat2((sfloat)inp.Joystick.x, sfloat.Zero);
        }
        
        public Dictionary _save_state ()
        {
            return new Dictionary
            {
                {"inp", i == null ? "" : JSON.Print(i)},
                { "tick", GetTree().Root.GetNode("SyncManager").Get("current_tick")},
                { "velocity", Velocity.SerializeToString() }
            };
        }

        public void _load_state (Dictionary state)
        {
            Velocity = sfloat2.FromString((string)state["velocity"]);
        }
    }
}