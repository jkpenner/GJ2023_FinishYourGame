using System;
using System.Text;
using Godot;

namespace SpaceEngineer
{
    public partial class UIShipStats : Label
    {
        private GameManager gameManager;

        private ShipController Ship => gameManager?.PlayerShip;

        public override void _Ready()
        {
            this.TryGetGameManager(out gameManager);
            UpdateText();
        }

        public override void _EnterTree()
        {
            GameEvents.ShipEnergyCapacityChanged.Connect(UpdateText);
            GameEvents.ShipEnergyUsageChanged.Connect(UpdateText);
            GameEvents.ShipEnergyOverloading.Connect(UpdateText);
            GameEvents.ShipEnergyOverloaded.Connect(UpdateText);
            GameEvents.ShipEnergyNormalized.Connect(UpdateText);
        }

        public override void _ExitTree()
        {
            GameEvents.ShipEnergyCapacityChanged.Disconnect(UpdateText);
            GameEvents.ShipEnergyUsageChanged.Disconnect(UpdateText);
            GameEvents.ShipEnergyOverloading.Disconnect(UpdateText);
            GameEvents.ShipEnergyOverloaded.Disconnect(UpdateText);
            GameEvents.ShipEnergyNormalized.Disconnect(UpdateText);
        }

        public override void _Process(double delta)
        {
            UpdateText();
        }

        private void UpdateText(int _) => UpdateText();

        private void UpdateText()
        {
            var builder = new StringBuilder();
            builder.Append("Energy\n");
            builder.Append($"Usage: {Ship.EnergyUsage}\n");
            builder.Append($"Capacity: {Ship.EnergyCapacity}\n");
            builder.Append($"Max Energy: {Ship.MaximumEnergy}\n");
            builder.Append($"Regen Rate: {Ship.GetEnergyRegenRate()}\n");
            builder.Append("\nOverload\n");
            builder.Append($"State: {Ship.OverloadState}\n");
            builder.Append($"Time Till Overload: {Ship.GetRemainingTimeTillOverload()}\n");
            builder.Append("\nSystems\n");
            builder.Append($"Weapons: {Ship.WeaponSystem.State}\n");
            builder.Append($"Shields: {Ship.ShieldSystem.State}\n");
            builder.Append($"Engines: {Ship.EngineSystem.State}\n");
            builder.Append($"Sensors: {Ship.SensorSystem.State}\n");
            Text = builder.ToString();
        }
    }
}