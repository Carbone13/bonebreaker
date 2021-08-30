using Godot;

public class FocusableButton : Button
{
    [Export] private bool OriginalButton;

    [Signal] public delegate void LoseFocus ();
    
    
    private NinePatchRect selecterRect;
    
    public override void _EnterTree ()
    {
        selecterRect = GetNode<NinePatchRect>("selecter");
        
        Connect("focus_entered", this, nameof(OnFocusGrab));
        Connect("focus_exited", this, nameof(OnFocusLose));
        Connect("mouse_entered", this, nameof(MouseEnter));
        Connect("mouse_exited", this, nameof(MouseLeave));
    }

    /// <summary>
    /// This is for global input, allow us to take the focus is their is currently none
    /// </summary>
    /// <param name="event"></param>
    public override void _Input (InputEvent @event)
    {
        if (!Visible) return;
        if (@event.IsActionPressed("ui_up") || @event.IsActionPressed("ui_left") ||
            @event.IsActionPressed("ui_down") || @event.IsActionPressed("ui_right"))
        {
            if (GetFocusOwner() == null && OriginalButton && !Disabled)
            {
                GrabFocus();
            }
        }
    }

    /// <summary>
    /// When we grab the Focus
    /// </summary>
    private void OnFocusGrab ()
    {
        if (!Visible) return;
        
        selecterRect.Visible = true;
    }

    /// <summary>
    /// When we lose the Focus
    /// </summary>
    private void OnFocusLose ()
    {
        if (!Visible) return;
        
        selecterRect.Visible = false;
        EmitSignal(nameof(LoseFocus));
    }

    /// <summary>
    /// If we are using the mouse, grab the focus
    /// </summary>
    private void MouseEnter ()
    {
        if (!Visible) return;
        
        GrabFocus();
    }

    /// <summary>
    /// If the mouse leave us, release the focus
    /// </summary>
    private void MouseLeave ()
    {
        if (!Visible) return;
        
        ReleaseFocus();
        EmitSignal(nameof(LoseFocus));
    }
}
