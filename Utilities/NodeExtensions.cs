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
            // gameManager = node.GetNode<GameManager>("%GameManager");
            gameManager = node.GetNode<GameManager>("/root/GameManager");
            if (gameManager is null)
            {
                GD.PrintErr("No GameManager found in the scene. Make sure a node name GameManager exists and is marked as a unique node.");
            }
            return gameManager is not null;
        }

        /// <summary>
        /// Attempt to find a parent node of a given type.
        /// </summary>
        public static T FindParentOfType<T>(this Node node) where T : Node
        {
            var parent = node.GetParent();
            if (parent is null)
            {
                return null;
            }

            if (parent is T typedParent)
            {
                return typedParent;
            }

            return parent.FindParentOfType<T>();
        }
    }
}