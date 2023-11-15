using Godot;

namespace SpaceEngineer
{
    [GlobalClass]
    public partial class SensorSystem : ShipSystem
    {
        public override ShipSystemType SystemType => ShipSystemType.Sensors;
    }
}