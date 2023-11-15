using Godot;

namespace SpaceEngineer
{
    public static class NodeExtensions
    {
        /// <summary>
        /// Attempts to gets the current GameManager in the scene.
        /// </summary>
        public static bool TryGetGameManager(this Node node, out GameManager gameManager)
        {
            gameManager = node.GetNode<GameManager>("%GameManager");
            if (gameManager is null)
            {
                GD.PrintErr("No GameManager found in the scene. Make sure a node name GameManager exists and is marked as a unique node.");
            }
            return gameManager is not null;
        }
    }
}