using Godot;

namespace SpaceEngineer
{
    [GlobalClass]
    public partial class EngineSystem : ShipSystem
    {
        public override ShipSystemType SystemType => ShipSystemType.Engines;
    }
}