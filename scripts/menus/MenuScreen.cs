using Godot;
using System;
using Nakama;

public class MenuScreen : Node
{
    [Export] private NodePath AccountName;

    private Label accountName;

    public override void _Ready ()
    {
        accountName = GetNode<Label>(AccountName);

        ISession session = Bonebreaker.Nakama.singleton.GetNakamaSession();
        if (session == null)
        {
            accountName.Text = "Offline";
        }
        else
        {
            accountName.Text = session.Username;
        }
        
    }
}
