using Godot;

namespace Bonebreaker.Scenes
{
    public class SceneManager : Node
    {
        public const string CONNECTING_SCENE = "res://scenes/menu/Connection Screen.tscn";
        public const string MAIN_MENU = "res://scenes/menu/Title Screen.tscn";
        public const string MATCHMAKING_MENU = "res://scenes/menu/Matchmaking Screen.tscn";
        public static SceneManager singleton;

        public override void _Ready ()
        {
            singleton = this;
        }

        public void LoadScene (string scene)
        {
            foreach (Node sceneRoot in GetTree().GetNodesInGroup("Scene Root"))
            {
                sceneRoot.QueueFree();
            }

            PackedScene prefab = ResourceLoader.Load<PackedScene>(scene);
            Node instance = prefab.Instance();
            
            GetTree().Root.AddChild(instance);
        }
    }
}