using Godot;

public class PasswordField : LineEdit
{
    private Button see_button;

    public override void _Ready ()
    {
        see_button = GetNode<Button>("see_button");
        see_button.Connect("button_down", this, nameof(SeeButtonDown));
        see_button.Connect("button_up", this, nameof(SeeButtonUp));
    }

    private void SeeButtonDown ()
    {
        this.Secret = false;
    }
    
    private void SeeButtonUp ()
    {
        this.Secret = true;
    }
}
