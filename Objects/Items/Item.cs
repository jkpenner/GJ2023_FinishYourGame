using Godot;

namespace SpaceEngineer
{
    [GlobalClass]
    public partial class Item : Resource
    {
        [Export] private AmmoType ammoType;
        [Export] private int shieldDamage;
        [Export] private int hullDamage;
        [Export] private float hitChance;
        [Export] private float peirceChance;

        [Export] private PackedScene visualScene;
        [Export] private PackedScene worldScene;

        public AmmoType AmmoType => ammoType;
        public int ShieldDamage => shieldDamage;
        public int HullDamage => hullDamage;
        public float HitChance => hitChance;
        public float PeirceChance => peirceChance;

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

        /// <summary>
        /// Creates an instance of the world interactable version of the item.
        /// </summary>
        public Node3D InstantiateWorld()
        {
            if (worldScene is null)
            {
                return null;
            }

            var instance = worldScene.Instantiate<WorldItem>();
            if (instance is null)
            {
                GD.PrintErr($"Item world scene is not a WorldItem");
                return null;
            }

            instance.SetItem(this);
            return instance;
        }
    }
}