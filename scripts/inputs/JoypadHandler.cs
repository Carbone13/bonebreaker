using System;
using Godot;

namespace Bonebreaker.Inputs
{
    public class JoypadHandler : InputHandler
    {
        public const float JUMP_BUFFER_MS = 70;
        public const float LIGHT_BUFFER_MS = 70;

        private uint lastJumpPressed, lastLightPressed;
        
        public int ID { get; set; }
        public readonly int ControllerID;
        
        public JoypadHandler (int id, int controllerId)
        {
            ID = id;
            ControllerID = controllerId;
        }
        
        public InputState GetState ()
        {
            return Poll();
        }
        
        public InputState Poll ()
        {
            InputState state = new InputState();
            state.DeviceID = ID;
            
            if (Godot.Input.IsJoyButtonPressed(ControllerID, (int) JoystickList.Button0))
            {
                state.Jump = true;
                lastJumpPressed = OS.GetTicksMsec();
            }
            if (!state.Jump && OS.GetTicksMsec() - lastJumpPressed < JUMP_BUFFER_MS)
            {
                state.Jump = true;
            }
            if (Godot.Input.IsJoyButtonPressed(ControllerID, (int) JoystickList.Button1) ||
                Godot.Input.IsJoyButtonPressed(ControllerID, (int) JoystickList.Button2))
            {
                state.Light = true;
                lastLightPressed = OS.GetTicksMsec();
            }
            
            if (!state.Light && (OS.GetTicksMsec() - lastLightPressed) < LIGHT_BUFFER_MS)
            {
                state.Light = true;
            }
            if (Godot.Input.IsJoyButtonPressed(ControllerID, (int) JoystickList.Button3))
            {
                state.Special = true;
            }
            if (Godot.Input.GetJoyAxis(ControllerID, (int)JoystickList.Axis1) > 0.4f)
            {
                state.Fall = true;
            }
            if (Math.Abs(Godot.Input.GetJoyAxis(ControllerID, (int)JoystickList.Axis0)) > 0.5f)
            {
                state.xInput = Math.Sign(Godot.Input.GetJoyAxis(ControllerID, (int)JoystickList.Axis0));
            }

            return state;
        }
    }
}