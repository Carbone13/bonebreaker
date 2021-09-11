using System.Drawing.Printing;
using Godot;
using Godot.Collections;

namespace Bonebreaker.Inputs
{
    public struct InputState
    {
        public int DeviceID;

        public bool Light, Special;
        public bool LightJustPressed;
        public Vector2 Joystick;
        public bool Jump, Fall, Dash;

        public override string ToString ()
        {
            return "DEVICE: " + DeviceID + " :: " + "light: " + Light + "  | " + "special: " + Special + "  | "+ "  | " + "jump: " + Jump + "  | " + "fall: " +
                   Fall;
        }

        public static InputState Deserialize (Dictionary input)
        {
            InputState state = new InputState();
            if (input.ToString() == "{}")
                return state;
            
            string joystick = (string)input[0];
            
            int x = int.Parse(joystick.Split('|')[0]);
            int y = int.Parse(joystick.Split('|')[1]);

            state.Joystick = new Vector2(x, y);
            state.Jump = (string)input[1] == "1";
            state.LightJustPressed = (string)input[2] == "1";
            
            return state;
        }

        public Dictionary Serialize ()
        {
            Dictionary dictionary = new Dictionary()
            {
                { 0, Joystick.x + "|" + Joystick.y },
                { 1, Jump ? "1" : "0" },
                { 2, LightJustPressed ? "1" : "0"}
            };

            return dictionary;
        }
    }
}