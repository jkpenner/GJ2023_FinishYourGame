using Godot;

namespace SpaceEngineer
{
    [GlobalClass]
    public partial class ShieldSystem : ShipSystem
    {
        public override ShipSystemType SystemType => ShipSystemType.Shields;
    }
}