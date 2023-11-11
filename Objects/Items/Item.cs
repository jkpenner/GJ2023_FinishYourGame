using Godot;

namespace SpaceEngineer
{
    [GlobalClass]
    public partial class Item : Resource
    {
        [Export] private PackedScene visualScene;

        public Node3D InstantiateVisual()
        {
            if (visualScene is null)
            {
                return null;
            }

            var instance = visualScene.Instantiate<Node3D>();
            if (instance is null)
            {
                GD.PrintErr($"Item visual scene is not a Node3D");
                return null;
            }

            return instance;
        }
    }
}