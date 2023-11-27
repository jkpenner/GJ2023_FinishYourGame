using Godot;

namespace SpaceEngineer
{
    [GlobalClass]
    public partial class EnemySpawnInfo : Resource
    {
        [Export] public bool WaitForAllEnemiesDefeated { get; set; }
        [Export] public float SpawnDelay { get; set; }
        [Export] public EnemyData Data { get; set; }
    }
}