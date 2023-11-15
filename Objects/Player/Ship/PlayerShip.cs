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

	public enum PlayerShipSystem
	{
		Engines,
		Sensors,

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
			Energy = energy;

			foreach (var system in systems)
			{
				system.Ship = this;
				system.StateChanged += OnSystemStateChanged;
				EnergyUsage += system.EnergyUsage;
			}
		}

		public override void _Process(double delta)
		{
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

		private void OnSystemStateChanged(ShipSystem system)
		{
			var newEnergyUsage = CalculateEnergyUsage();
			// if (EnergyUsage == newEnergyUsage)
			// {
			// 	return;
			// }

			EnergyUsage = newEnergyUsage;

			GD.Print($"Energy Usage: {EnergyUsage}, Energy Capacity: {Energy}");

			if (EnergyUsage > Energy && OverloadState == EnergyOverloadState.NotOverloaded)
			{
				GD.Print($"Ship energy is overloading ({overloadDelay} seconds)");
				OverloadState = EnergyOverloadState.Overloading;
				overloadCounter = 0f;

				GameEvents.ShipEnergyOverloading.Emit(this);
			}
			else if (EnergyUsage <= Energy && OverloadState == EnergyOverloadState.Overloading)
			{
				GD.Print("Ship energy returned to normal");
				OverloadState = EnergyOverloadState.NotOverloaded;
				overloadCounter = 0f;

				GameEvents.ShipEnergyNormalized.Emit(this);
			}
		}

		private void OnOverloadEvent()
		{
			GD.Print("Ship overloaded!");
			GameEvents.ShipEnergyOverloaded.Emit(this);

			int energyUsage = CalculateEnergyUsage();

			// All overclocked systems are destroyed when an overload event occurs.
			foreach (var system in systems.Where(s => s.State == ShipSystemState.Overclocked))
			{
				energyUsage -= system.EnergyUsage;
				system.Destroy();
			}

			// If ship is still overloaded afterwards, randomly disable other systems
			if (energyUsage > Energy)
			{
				var poweredSystems = systems.Where((s) =>
					s.State == ShipSystemState.Powered ||
					s.State == ShipSystemState.Damaged
				).ToList();

				Random random = new Random();
				for (int i = poweredSystems.Count; i > 0; i--)
				{
					int index = random.Next(i);
					var system = poweredSystems[index];
					poweredSystems.RemoveAt(index);

					if (system.State == ShipSystemState.Powered)
					{
						energyUsage -= system.EnergyUsage;
						system.TogglePower();
					}
					// Damage systems are just destroyed
					else
					{
						energyUsage -= system.EnergyUsage;
						system.Destroy();
					}

					if (energyUsage <= Energy)
					{
						// Energy normalized exit
						break;
					}
				}
			}

			// Stop overload event and reduce energy capacity by 1
			OverloadState = EnergyOverloadState.NotOverloaded;
			EnergyUsage = CalculateEnergyUsage();
			Energy = Mathf.Max(Energy - 1, 0);
			GameEvents.ShipEnergyCapacityChanged.Emit(this);

			GD.Print("Ship energy returned to normal");
			GameEvents.ShipEnergyNormalized.Emit(this);
		}

		/// <summary>
		/// Get the total energy usage of all systems.
		/// </summary>
		public int CalculateEnergyUsage()
		{
			return systems.Sum(s => s.EnergyUsage);
		}

		public ShipSystem GetSystem(ShipSystemType systemType)
		{
			foreach (var system in systems)
			{
				if (system.SystemType == systemType)
				{
					return system;
				}
			}

			return null;
		}
	}
}