using Godot;

namespace Bonebreaker.Inputs
{
    public class KeyboardHandler : InputHandler
    {
        public const float JUMP_BUFFER_MS = 100;
        public const float LIGHT_BUFFER_MS = 100;

        private uint lastJumpPressed, lastLightPressed;
        
        public int socd = 0;
        public int ID { get; set; }

        private InputState previousState;

        private bool Left, Right;

        public KeyboardHandler (int id)
        {
            ID = id;
        }

        public InputState GetState ()
        {
            return Poll();
        }
        
        public InputState Poll ()
        {
            string keyboard = OS.GetLatinKeyboardVariant();

            if (keyboard == "AZERTY")
            {
                return PollAZERTY();
            }
            else
            {
                return PollQWERTY();
            }
        }
        
        private InputState PollQWERTY ()
        {
            InputState state = new InputState();
            state.DeviceID = ID;
            
            if (Godot.Input.IsKeyPressed((int)KeyList.Space) ||
                Godot.Input.IsKeyPressed((int)KeyList.W) ||
                Godot.Input.IsKeyPressed((int)KeyList.Up))
            {
                state.Jump = true;
                lastJumpPressed = (uint)OS.GetTicksMsec();
            }
            if (!state.Jump && lastJumpPressed - OS.GetTicksMsec() < JUMP_BUFFER_MS)
            {
                state.Jump = true;
            }

            if (Godot.Input.IsKeyPressed((int) KeyList.E))
            {
                state.Light = true;
                lastLightPressed = (uint)OS.GetTicksMsec();
            }
            if (!state.Light && (OS.GetTicksMsec() - lastLightPressed) < LIGHT_BUFFER_MS)
            {
                state.Light = true;
            }

            if (Godot.Input.IsKeyPressed((int) KeyList.Q))
            {
                state.Special = true;
            }

            if (Godot.Input.IsKeyPressed((int)KeyList.Control))
            {
                state.Dash = true;
            }
            
            if (Godot.Input.IsKeyPressed((int)KeyList.Shift) ||
                Godot.Input.IsKeyPressed((int)KeyList.S) ||
                Godot.Input.IsKeyPressed((int)KeyList.Down))
            {
                state.Fall = true;
            }
            
            bool leftPressedThisFrame = Godot.Input.IsKeyPressed((int) KeyList.A) ||
                                        Godot.Input.IsKeyPressed((int) KeyList.Left);
            bool rightPressedThisFrame = Godot.Input.IsKeyPressed((int) KeyList.D) ||
                                         Godot.Input.IsKeyPressed((int) KeyList.Right);
            bool upPressedThisFrame = Godot.Input.IsKeyPressed((int) KeyList.W) ||
                                        Godot.Input.IsKeyPressed((int) KeyList.Up);
            bool downPressedThisFrame = Godot.Input.IsKeyPressed((int) KeyList.S) ||
                                         Godot.Input.IsKeyPressed((int) KeyList.Down);

            state.Joystick.y += upPressedThisFrame ? -1 : 0 + (downPressedThisFrame ? 1 : 0);
            if (socd == 0)
            {
                if (leftPressedThisFrame)
                    state.Joystick.x -= 1;
                if (rightPressedThisFrame)
                    state.Joystick.x += 1;
            }
            if (socd != 0)
            {
                bool leftJustPressed = !Left && leftPressedThisFrame;
                bool rightJustPressed = !Right && rightPressedThisFrame;

                if (!leftPressedThisFrame && !rightPressedThisFrame)
                    state.Joystick.x = 0;
                else
                    state.Joystick.x = previousState.Joystick.x;
                if (leftJustPressed)
                    state.Joystick.x = -1;
                if (rightJustPressed)
                    state.Joystick.x = 1;
                
                Left = leftPressedThisFrame;
                Right = rightPressedThisFrame;
            }

            previousState = state;
            return state;
        }

        private InputState PollAZERTY ()
        {
            InputState state = new InputState();
            state.DeviceID = ID;
            
            if (Godot.Input.IsKeyPressed((int)KeyList.Space) ||
                Godot.Input.IsKeyPressed((int)KeyList.Z) ||
                Godot.Input.IsKeyPressed((int)KeyList.Up))
            {
                state.Jump = true;
            }
            if (!state.Jump && lastJumpPressed - OS.GetTicksMsec() < JUMP_BUFFER_MS)
            {
                state.Jump = true;
            }

            if (Godot.Input.IsKeyPressed((int) KeyList.E))
            {
                state.Light = true;
                lastLightPressed = (uint)OS.GetTicksMsec();
            }
            if (!state.Light && (OS.GetTicksMsec() - lastLightPressed) < LIGHT_BUFFER_MS)
            {
                state.Light = true;
            }

            if (Godot.Input.IsKeyPressed((int) KeyList.A))
            {
                state.Special = true;
            }
            
            if (Godot.Input.IsKeyPressed((int)KeyList.Shift) ||
                Godot.Input.IsKeyPressed((int)KeyList.S) ||
                Godot.Input.IsKeyPressed((int)KeyList.Down))
            {
                state.Fall = true;
            }
            
            if (Godot.Input.IsKeyPressed((int)KeyList.Control))
            {
                state.Dash = true;
            }

            bool leftPressedThisFrame = Godot.Input.IsKeyPressed((int) KeyList.Q) ||
                                        Godot.Input.IsKeyPressed((int) KeyList.Left);
            bool rightPressedThisFrame = Godot.Input.IsKeyPressed((int) KeyList.D) ||
                                         Godot.Input.IsKeyPressed((int) KeyList.Right);
            bool upPressedThisFrame = Godot.Input.IsKeyPressed((int) KeyList.Z) ||
                                      Godot.Input.IsKeyPressed((int) KeyList.Up);
            bool downPressedThisFrame = Godot.Input.IsKeyPressed((int) KeyList.S) ||
                                        Godot.Input.IsKeyPressed((int) KeyList.Down);

            state.Joystick.y += upPressedThisFrame ? -1 : 0 + (downPressedThisFrame ? 1 : 0);
            if (socd == 0)
            {
                if (leftPressedThisFrame)
                    state.Joystick.x -= 1;
                if (rightPressedThisFrame)
                    state.Joystick.x += 1;
            }
            if (socd != 0)
            {
                bool leftJustPressed = !Left && leftPressedThisFrame;
                bool rightJustPressed = !Right && rightPressedThisFrame;

                if (!leftPressedThisFrame && !rightPressedThisFrame)
                    state.Joystick.x = 0;
                else
                    state.Joystick.x = previousState.Joystick.x;
                if (leftJustPressed)
                    state.Joystick.x = -1;
                if (rightJustPressed)
                    state.Joystick.x = 1;
                
                Left = leftPressedThisFrame;
                Right = rightPressedThisFrame;
            }

            previousState = state;
            return state;
        }
    }
}