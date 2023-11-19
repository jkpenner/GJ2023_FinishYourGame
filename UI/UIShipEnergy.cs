using Godot;
using System;

namespace SpaceEngineer
{
	public partial class UIShipEnergy : Control
	{
		[Export] PackedScene cellScene;

        public override void _EnterTree()
        {
            GameEvents.ShipEnergyCapacityChanged.Connect(OnCapacityChanged);
			GameEvents.ShipEnergyNormalized.Connect(OnEnergyNormalized);
			GameEvents.ShipEnergyOverloading.Connect(OnEnergyOverloading);
			GameEvents.ShipEnergyUsageChanged.Connect(OnUsageChanged);
        }

        public override void _ExitTree()
        {
            GameEvents.ShipEnergyCapacityChanged.Disconnect(OnCapacityChanged);
			GameEvents.ShipEnergyNormalized.Disconnect(OnEnergyNormalized);
			GameEvents.ShipEnergyOverloading.Disconnect(OnEnergyOverloading);
			GameEvents.ShipEnergyUsageChanged.Disconnect(OnUsageChanged);
        }

		private void OnCapacityChanged(int obj)
        {
            throw new NotImplementedException();
        }


        private void OnEnergyNormalized()
        {
            throw new NotImplementedException();
        }


        private void OnEnergyOverloading()
        {
            throw new NotImplementedException();
        }


        private void OnUsageChanged(int obj)
        {
            throw new NotImplementedException();
        }
    }
}
