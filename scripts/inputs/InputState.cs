using Godot;

namespace Bonebreaker.Inputs
{
    public struct InputState
    {
        public int DeviceID;

        public bool Light, Special;
        public Vector2 Joystick;
        public bool Click;
        public bool Jump, Fall, Dash;

        public override string ToString ()
        {
            return "DEVICE: " + DeviceID + " :: " + "light: " + Light + "  | " + "special: " + Special + "  | "+ "  | " + "jump: " + Jump + "  | " + "fall: " +
                   Fall;
        }
    }
}