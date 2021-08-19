using Godot;
using System;
using Bonebreaker.Inputs;
using Input = Bonebreaker.Inputs.Input;

public class Menu : Node
{
    public static Menu sg;
    
    [Signal] public delegate void Click ();
    [Signal] public delegate void Release ();
    [Signal] public delegate void SelecterShow (bool value);
    
    public Vector2 Position;
    public bool ShowSelecter;

    private bool clickedLastFrame = false;

    public override void _EnterTree ()
    {
        sg = this;
    }

    public override void _Process (float delta)
    {
        var primaryDevice = Input.singleton.GetPrimaryInputHandler();
        
        if (primaryDevice is JoypadHandler != ShowSelecter)
        {
            ShowSelecter = primaryDevice is JoypadHandler;
            EmitSignal(nameof(SelecterShow), ShowSelecter);
        }
        
        var input = Input.singleton.GetStateOfPrimaryDevice();
        
        if (input.Click && !clickedLastFrame)
        {
            EmitSignal(nameof(Click));
            clickedLastFrame = true;
        }

        if (!input.Click && clickedLastFrame)
        {
            clickedLastFrame = false;
            EmitSignal(nameof(Release));
        }
    }
}
