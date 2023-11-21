using Godot;
using Godot.Collections;

namespace SpaceEngineer
{
    [GlobalClass]
    public partial class GameEncounter : Resource
    {
        [Export] Array<EnemySpawnInfo> enemySpawns;
        [Export] Array<Item> initialItems;

        public Array<EnemySpawnInfo> EnemySpawns => enemySpawns;
        public Array<Item> InitialItems => initialItems;
    }
}