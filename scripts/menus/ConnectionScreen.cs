using Godot;
using System;
using Bonebreaker.Scenes;
using Godot.Collections;
using Nakama;

public class ConnectionScreen : TextureRect
{
    private const string CREDENTIALS_PATH = "user://credentials.json";
    
    [Export] private NodePath LoginParent;
    [Export] private NodePath RegisterParent;
    [Export] private NodePath ConnectingParent;
    [Export] private NodePath LoginEmailField;
    [Export] private NodePath LoginPasswordField;
    [Export] private NodePath LoginSaveCredentials;
    [Export] private NodePath RegisterEmailField;
    [Export] private NodePath RegisterUsernameField;
    [Export] private NodePath RegisterPasswordField;
    [Export] private NodePath ErrorDialog;

    private AcceptDialog dialog;
    
    private Control loginParentNode;
    private Control registerParentNode;
    private Control connectingScreen;

    private LineEdit loginPasswordField, loginEmailField;
    private CheckButton loginSaveCredentials;


    private string _email, _password;
    
    public override void _Ready ()
    {
        loginParentNode = GetNode<Control>(LoginParent);
        registerParentNode = GetNode<Control>(RegisterParent);
        connectingScreen = GetNode<Control>(ConnectingParent);

        loginPasswordField = GetNode<LineEdit>(LoginPasswordField);
        loginEmailField = GetNode<LineEdit>(LoginEmailField);
        loginSaveCredentials = GetNode<CheckButton>(LoginSaveCredentials);

        dialog = GetNode<AcceptDialog>(ErrorDialog);

        File credentials = new File();
        if (credentials.FileExists(CREDENTIALS_PATH))
        {
            credentials.Open(CREDENTIALS_PATH, File.ModeFlags.Read);
            Dictionary result = JSON.Parse(credentials.GetAsText()).Result as Dictionary;

            _email = (string)result["email"];
            _password = (string)result["password"];
            
            loginEmailField.Text = _email;
            loginPasswordField.Text = _password;
        }
        
        credentials.Close();
    }
    
    private void SaveCredentials ()
    {
        File credential = new File();
        credential.Open(CREDENTIALS_PATH, File.ModeFlags.Write);

        Dictionary credentials = new Dictionary {{"email", _email}, {"password", _password}};
        credential.StoreLine(JSON.Print(credentials));
        credential.Close();
    }

    public async void Login (bool saveCredentials = false)
    {
        connectingScreen.Visible = true;

        ISession session; 
        
        try
        {
            session = await Bonebreaker.Nakama.singleton.GetNakamaClient()
                .AuthenticateEmailAsync(_email, _password, null, false);
        }
        catch (ApiResponseException e)
        {
            connectingScreen.Visible = false;
            dialog.DialogText = "Error while connecting,  ERROR " + e.StatusCode;
            dialog.DialogText += "\n" + e.Message;
            dialog.Show();
            
            return;
        }
        

        if (session.IsExpired)
        {
            connectingScreen.Visible = false;
        }
        else
        {
            if(saveCredentials)
                SaveCredentials();
            
            Bonebreaker.Nakama.singleton.SetNakamaSession(session);
            
            SceneManager.singleton.LoadScene(SceneManager.MAIN_MENU);
        }
    }

    public void Offline ()
    {
        SceneManager.singleton.LoadScene(SceneManager.MAIN_MENU);
    }

    public void ConnectPressed ()
    {
        _email = loginEmailField.Text.Trim();
        _password = loginPasswordField.Text.Trim();
        
        Login(loginSaveCredentials.Pressed);
    }

    public void RegisterPressed () {}
    
    public void SwitchToLoginSide ()
    {
        GD.Print("Login Side");
        
        AtlasTexture atlas = Texture as AtlasTexture;
        atlas.Region = new Rect2(Vector2.Zero, new Vector2(139, 157));

        registerParentNode.Visible = false;
        loginParentNode.Visible = true;
    }

    public void SwitchToRegisterSide ()
    {
        GD.Print("Register Side");
        AtlasTexture atlas = Texture as AtlasTexture;
        atlas.Region = new Rect2(new Vector2(139, 0), new Vector2(139, 157));

        loginParentNode.Visible = false;
        registerParentNode.Visible = true;
    }

}
