namespace Bonebreaker.Inputs
{
    public struct InputState
    {
        public int DeviceID;

        public bool Light, Special;
        public int xInput;
        public bool Jump, Fall, Dash;

        public override string ToString ()
        {
            return "DEVICE: " + DeviceID + " :: " + "light: " + Light + "  | " + "special: " + Special + "  | " + "xInput: " + xInput + "  | " + "jump: " + Jump + "  | " + "fall: " +
                   Fall;
        }
    }
}