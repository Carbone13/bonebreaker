using Godot;
using System;

public class TextButton : FocusableButton
{
    [Export] public Color Normal, Hovered, Clicked;
    private new Label Text;

    public override void _Ready ()
    {
        Text = GetNode<Label>("text");

        Connect("button_down", this, nameof(MouseDown));
        Connect("button_up", this, nameof(MouseUp));
        Connect("mouse_entered", this, nameof(MouseIn));
        Connect("mouse_exited", this, nameof(MouseOut));
    }
    
    private void MouseIn ()
    {
        Text.AddColorOverride("font_color", Hovered);
        StyleBoxFlat stylebox = Text.GetChild<Panel>(0).GetStylebox("panel") as StyleBoxFlat;
        stylebox.BgColor = Hovered;
        
        Text.GetChild<Panel>(0).AddStyleboxOverride("panel", stylebox);
        Text.GetChild<Panel>(1).AddStyleboxOverride("panel", stylebox);
    }
    
    private void MouseOut ()
    {
        Text.AddColorOverride("font_color", Normal);
        StyleBoxFlat stylebox = Text.GetChild<Panel>(0).GetStylebox("panel") as StyleBoxFlat;
        stylebox.BgColor = Normal;
        
        Text.GetChild<Panel>(0).AddStyleboxOverride("panel", stylebox);
        Text.GetChild<Panel>(1).AddStyleboxOverride("panel", stylebox);
    }
    
    private void MouseDown ()
    {
        Text.AddColorOverride("font_color", Clicked);
        StyleBoxFlat stylebox = Text.GetChild<Panel>(0).GetStylebox("panel") as StyleBoxFlat;
        stylebox.BgColor = Clicked;
        
        Text.GetChild<Panel>(0).AddStyleboxOverride("panel", stylebox);
        Text.GetChild<Panel>(1).AddStyleboxOverride("panel", stylebox);
    }
    
    private void MouseUp ()
    {
        Text.AddColorOverride("font_color", Hovered);
        StyleBoxFlat stylebox = Text.GetChild<Panel>(0).GetStylebox("panel") as StyleBoxFlat;
        stylebox.BgColor = Hovered;
        
        Text.GetChild<Panel>(0).AddStyleboxOverride("panel", stylebox);
        Text.GetChild<Panel>(1).AddStyleboxOverride("panel", stylebox);
    }
}
