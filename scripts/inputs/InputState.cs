using System.Drawing.Printing;
using Godot;
using Godot.Collections;

namespace Bonebreaker.Inputs
{
    public struct InputState
    {
        public int DeviceID;

        public Vector2 Joystick;
        public bool Light, Special;
        public bool Jump, Fall, Dash;

        public override string ToString ()
        {
            return "DEVICE: " + DeviceID + " :: " + "light: " + Light + "  | " + "special: " + Special + "  | "+ "  | " + "jump: " + Jump + "  | " + "fall: " +
                   Fall;
        }

        // TODO serialize everything + add light buffer
        public static InputState Deserialize (Dictionary input)
        {
            InputState state = new InputState();
            if (input.ToString() == "{}")
                return state;

            state.Joystick = new Vector2((float)input["x"], (float)input["y"]);
            byte buttons = (byte) (int)input["b"];
            
            state.Jump = (0b10000000 & buttons) != 0;
            state.Light = (0b01000000 & buttons) != 0;
            state.Fall = (0b00100000 & buttons) != 0;
            state.Dash = (0b00010000 & buttons) != 0;
            state.Special = (0b00001000 & buttons) != 0;
            
            return state;
        }

        public Dictionary Serialize ()
        {
            byte buttons = 0b00000000;
            
            if (Jump)
                buttons |= 0b10000000;
            if(Light)
                buttons |= 0b01000000;
            if(Fall)
                buttons |= 0b00100000;
            if(Dash)
                buttons |= 0b00010000;
            if(Special)
                buttons |= 0b00001000;
            
            Dictionary dictionary = new Dictionary
            {
                { "x", Joystick.x},
                { "y", Joystick.y},
                { "b", buttons }
            };
            
            return dictionary;
        }
    }
    
    
}