using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Bonebreaker.Inputs
{
    public class Input : Node
    {
        public static Input singleton;
        
        private KeyboardHandler _keyboardHandler = new KeyboardHandler(0);
        private Dictionary<int, JoypadHandler> _joypadHandlers = new Dictionary<int, JoypadHandler>();

        private InputHandler _primaryHandler;

        [Signal] public delegate void OnNewControllerConnected (int id);

        public override void _Ready ()
        {
            singleton = this;
            _primaryHandler = _keyboardHandler;
        }

        public InputState GetStateFromID (int id)
        {
            if (id == -1)
            {
                return GetStateOfPrimaryDevice();
            }
            if (id == 0)
            {
                return _keyboardHandler.GetState();
            }
            else
            {
                return _joypadHandlers[id].GetState();
            }
        }

        public InputState GetStateOfPrimaryDevice ()
        {
            return _primaryHandler.GetState();
        }

        public override void _UnhandledInput (InputEvent @event)
        {
            if (@event is InputEventJoypadMotion || @event is InputEventJoypadButton)
            {
                if (!IsJoypadRegistered(@event.Device))
                {
                    EmitSignal(nameof(OnNewControllerConnected), _joypadHandlers.Values.Count + 1);
                    _joypadHandlers.Add(@event.Device, new JoypadHandler(_joypadHandlers.Values.Count + 1, @event.Device));
                }

                _primaryHandler = _joypadHandlers[@event.Device];
            }
            else if (@event is InputEventKey || @event is InputEventMouse)
            {
                _primaryHandler = _keyboardHandler;
            }
        }
        
        private bool IsJoypadRegistered (int id)
        {
            foreach (JoypadHandler handler in _joypadHandlers.Values)
            {
                if (handler.ControllerID == id)
                {
                    return true;
                }
            }

            return false;
        }
    }
}

