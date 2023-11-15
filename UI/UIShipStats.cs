using System;
using Godot;

namespace SpaceEngineer
{
    public partial class UIShipStats : Control
    {
        [Export] Label energyLabel;
        [Export] Label overloadStateLabel;
        [Export] Label overloadTimeLabel;

        private GameManager gameManager;

        public override void _Ready()
        {
            gameManager = GetNode<GameManager>("%GameManager");

        }

        public override void _EnterTree()
        {
            GameEvents.ShipEnergyCapacityChanged.Connect(OnShipEnergyChanged);
            GameEvents.ShipEnergyUsageChanged.Connect(OnShipEnergyChanged);
            GameEvents.ShipEnergyOverloading.Connect(OnShipOverloadStateChanged);
            GameEvents.ShipEnergyOverloaded.Connect(OnShipOverloadStateChanged);
            GameEvents.ShipEnergyNormalized.Connect(OnShipOverloadStateChanged);
        }

        public override void _ExitTree()
        {
            GameEvents.ShipEnergyCapacityChanged.Disconnect(OnShipEnergyChanged);
            GameEvents.ShipEnergyUsageChanged.Disconnect(OnShipEnergyChanged);
            GameEvents.ShipEnergyOverloading.Disconnect(OnShipOverloadStateChanged);
            GameEvents.ShipEnergyOverloaded.Disconnect(OnShipOverloadStateChanged);
            GameEvents.ShipEnergyNormalized.Disconnect(OnShipOverloadStateChanged);
        }

        public override void _Process(double delta)
        {
            if (gameManager.PlayerShip is not null)
            {
                if (gameManager.PlayerShip.OverloadState == EnergyOverloadState.Overloading)
                {
                    overloadTimeLabel.Text = $"Time till overload: {gameManager.PlayerShip.TimeTillOverload}";
                }
                else
                {
                    overloadTimeLabel.Text = "Time till overload: N/a";
                }
            }
        }

        private void OnShipEnergyChanged(PlayerShip ship)
        {
            energyLabel.Text = $"Energy Usage: {ship.EnergyUsage}, Energy Capacity: {ship.Energy}";
        }

        private void OnShipOverloadStateChanged(PlayerShip ship)
        {
            overloadStateLabel.Text = $"Overload State: {ship.OverloadState.ToString()}";
        }
    }
}