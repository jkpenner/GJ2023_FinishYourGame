using Godot;
using System;
namespace SpaceEngineer
{
	public enum UIShipEnergyCellState
	{
		Enactive,
		Active,
		Overloaded,
		Depleted
	}

	public partial class UIShipEnergyCell : Panel
	{
		[Export] Color enactive = new Color("#004053");
		[Export] Color active = new Color("#00a9d6");
		[Export] Color overloaded = new Color("#ff9900");
		[Export] Color depleted = new Color("#990000");

		private UIShipEnergyCellState state;

		public void SetState(UIShipEnergyCellState state)
		{
			Modulate = state switch
			{
				UIShipEnergyCellState.Active => active,
				UIShipEnergyCellState.Enactive => enactive,
				UIShipEnergyCellState.Overloaded => overloaded,
				UIShipEnergyCellState.Depleted => depleted,
				_ => new Color("#000000")
			};
		}
	}
}