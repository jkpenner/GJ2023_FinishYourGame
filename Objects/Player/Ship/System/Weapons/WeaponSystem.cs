using Godot;

namespace SpaceEngineer
{
    [GlobalClass]
    public partial class WeaponSystem : ShipSystem
    {
        public override ShipSystemType SystemType => ShipSystemType.Weapons;
    }
}