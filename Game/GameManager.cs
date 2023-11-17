using System;
using Godot;

namespace SpaceEngineer
{
    public partial class GameManager : Node
    {
        [Export] ShipController playerShip;


        public ShipController PlayerShip => playerShip;

        public override void _Ready()
        {
            if (PlayerShip is not null)
            {
                PlayerShip.SystemStateChanged += GameEvents.ShipSystemStateChanged.Emit;
                PlayerShip.EnergyUsageChanged += GameEvents.ShipEnergyUsageChanged.Emit;
                PlayerShip.EnergyCapacityChanged += GameEvents.ShipEnergyCapacityChanged.Emit;

                PlayerShip.Overloading += GameEvents.ShipEnergyOverloaded.Emit;
                PlayerShip.OverloadEventStarted += GameEvents.ShipEnergyOverloaded.Emit;
                PlayerShip.EnergyNormalized += GameEvents.ShipEnergyNormalized.Emit;
            }
        }

        public override void _ExitTree()
        {
            if (PlayerShip is not null)
            {
                PlayerShip.SystemStateChanged -= GameEvents.ShipSystemStateChanged.Emit;
                PlayerShip.EnergyUsageChanged -= GameEvents.ShipEnergyUsageChanged.Emit;
                PlayerShip.EnergyCapacityChanged -= GameEvents.ShipEnergyCapacityChanged.Emit;

                PlayerShip.Overloading -= GameEvents.ShipEnergyOverloaded.Emit;
                PlayerShip.OverloadEventStarted -= GameEvents.ShipEnergyOverloaded.Emit;
                PlayerShip.EnergyNormalized -= GameEvents.ShipEnergyNormalized.Emit;
            }
        }
    }
}