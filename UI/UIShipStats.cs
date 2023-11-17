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

        private ShipController Ship => gameManager?.PlayerShip;

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
            if (Ship is not null && Ship.OverloadState == ShipOverloadState.Overloading)
            {
                overloadTimeLabel.Text = $"Time till overload: {Ship.GetRemainingTimeTillOverload()}";
            }
            else
            {
                overloadTimeLabel.Text = "Time till overload: N/a";
            }
        }

        private void OnShipEnergyChanged(int _)
        {
            if (Ship is null)
            {
                energyLabel.Text = "Energy Usage: Unknown, Energy Capacity: Unknown";
                return;
            }

            energyLabel.Text = $"Energy Usage: {Ship.EnergyUsage}, Energy Capacity: {Ship.EnergyCapacity}";
        }

        private void OnShipOverloadStateChanged()
        {
            if (Ship is null)
            {
                overloadStateLabel.Text = "Overload State: Unknown";
                return;
            }

            overloadStateLabel.Text = $"Overload State: {Ship.OverloadState}";
        }
    }
}