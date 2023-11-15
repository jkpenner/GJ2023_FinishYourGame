using System;
using System.Linq;
using Godot;

namespace SpaceEngineer
{
	public enum EnergyOverloadState
	{
		NotOverloaded,
		Overloading,
		Overloaded,
	}

	[GlobalClass]
	public partial class PlayerShip : Node3D
	{
		[Export] int energy = 10;
		[Export] float overloadDelay = 5f;
		[Export] Godot.Collections.Array<ShipSystem> systems;

		private float overloadCounter = 0f;

		/// <summary>
		/// The maximum amount of energy the ship can store.
		/// </summary>
		public int MaxEnergy => energy;

		/// <summary>
		/// The current usable amount of energy.
		/// </summary>
		public int Energy { get; private set; }

		/// <summary>
		/// The amount of energy being used by all ship systems.
		/// </summary>
		public int EnergyUsage { get; private set; }

		public EnergyOverloadState OverloadState { get; private set; }

		/// <summary>
		/// The amount of time in seconds till the ships batteries overload.
		/// </summary>
		public float TimeTillOverload => Mathf.Max(overloadDelay - overloadCounter, 0f);

		public override void _Ready()
		{
			foreach (var system in systems)
			{
				system.Ship = this;
			}
		}

		public override void _Process(double delta)
		{
			// Todo: Eventually move to an event based approach to check
			if (UpdateEnergyUsage())
			{
				GD.Print($"Energy Usage: {EnergyUsage}, Energy Capacity: {Energy}");

				if (EnergyUsage > Energy && OverloadState == EnergyOverloadState.NotOverloaded)
				{
					GD.Print($"Ship energy is overloading ({overloadDelay} seconds)");
					OverloadState = EnergyOverloadState.Overloading;
					overloadCounter = 0f;
				}
				else if (EnergyUsage <= Energy && OverloadState == EnergyOverloadState.Overloading)
				{
					GD.Print("Ship energy returned to normal");
					OverloadState = EnergyOverloadState.NotOverloaded;
					overloadCounter = 0f;
				}
			}

			if (OverloadState == EnergyOverloadState.Overloading)
			{
				overloadCounter += (float)delta;
				if (overloadCounter >= overloadDelay)
				{
					OverloadState = EnergyOverloadState.Overloaded;
					OnOverloadEvent();
				}
			}
		}

		private void OnOverloadEvent()
        {
			GD.Print("Ship overloaded!");

			// All overclocked systems are destroyed when an overload event occurs.
            foreach(var system in systems)
			{
				if (system.State == SystemState.Overclocked)
				{
					system.Destroy();
				}
			}

			// If ship is still overloaded afterwards, randomly disable other systems
			if (UpdateEnergyUsage() && EnergyUsage > Energy)
			{
				var remainingSystems = systems.Where((s) => s.PowerUsage > 0).ToList();

				Random random = new Random();
				for (int i = remainingSystems.Count; i > 0; i--)
				{
					int index = random.Next(i);
					remainingSystems[index].Destroy();
					remainingSystems.RemoveAt(index);

					if (UpdateEnergyUsage() && EnergyUsage <= Energy)
					{
						// Energy normalized exit
						break;
					}
				}
			}

			// Stop overload event and reduce energy capacity by 1
			OverloadState = EnergyOverloadState.NotOverloaded;
			Energy = Mathf.Max(Energy - 1, 0);

			GD.Print("Ship energy returned to normal");
        }

        /// <summary>
        /// Updates the current value for system energy usage. Returns
        /// true if the value changed.
        /// </summary>
        /// <returns></returns>
        private bool UpdateEnergyUsage()
		{
			int energyUsage = 0;
			foreach (var system in systems)
			{
				energyUsage += system.PowerUsage;
			}

			if (EnergyUsage != energyUsage)
			{
				EnergyUsage = energyUsage;
				return true;
			}

			return false;
		}
	}
}