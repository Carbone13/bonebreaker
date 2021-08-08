using Godot;
using System;
using Bonebreaker.Net;

public class ConnectionScreen : TextureRect
{
    private LineEdit RegisterUsernameField, RegisterEmailField, RegisterPasswordField;
    private LineEdit LoginEmailField, LoginPasswordField;
    private Control LoginPanel, RegisterPanel;

    public override void _Ready ()
    {
        LoginPanel = GetNode<Control>("Login");
        RegisterPanel = GetNode<Control>("Register");
        
        RegisterUsernameField = GetNode<LineEdit>("Register/Username Field");
        RegisterEmailField = GetNode<LineEdit>("Register/Email Field");
        RegisterPasswordField = GetNode<LineEdit>("Register/Password Field");
        
        LoginEmailField = GetNode<LineEdit>("Login/Email Field");
        LoginPasswordField = GetNode<LineEdit>("Login/Password Field");
    }

    public async void ConnectPressed ()
    {
        LoginAccount loginQuery = new LoginAccount();
        loginQuery.Credential = LoginEmailField.Text;
        loginQuery.Password = LoginPasswordField.Text;
        GD.Print("labite");
        AccountLoginResult result = await Network.TryLogin(loginQuery.Credential, loginQuery.Password);
        if(result != null)
            GD.Print("Successfully connected, here is our username: " + result.Account.Username);
    }

    public async void RegisterPressed ()
    {
        if (!IsEmailValid(RegisterEmailField.Text)) return;
        if (!IsUsernameValid(RegisterUsernameField.Text)) return;

        await Network.CreateAccount(new Account
        {
            Email = RegisterEmailField.Text.Trim(), Password = RegisterPasswordField.Text.Trim(),
            Username = RegisterUsernameField.Text.Trim()
        });
    }

    public bool IsEmailValid (string txt)
    {
        return txt.Contains("@");
    }

    public bool IsUsernameValid (string txt)
    {
        return !txt.Contains("@");
    }

    public void SwitchToLoginSide ()
    {
        LoginPanel.Visible = true;
        RegisterPanel.Visible = false;

        AtlasTexture texture = Texture as AtlasTexture;
        texture.Region = new Rect2(0, 0, 139, 157);
    }

    public void SwitchToRegisterSide ()
    {
        LoginPanel.Visible = false;
        RegisterPanel.Visible = true;
        
        AtlasTexture texture = Texture as AtlasTexture;
        texture.Region = new Rect2(139, 0, 139, 157);
    }
}
