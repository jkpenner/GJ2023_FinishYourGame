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
		private TextureProgressBar fill;

        public override void _Ready()
        {
			fill = GetNode<TextureProgressBar>("Fill");
            fill.Modulate = active;
			fill.Value = 0f;
        }

		public void SetRechargePercent(float percent)
		{
			fill.Value = Mathf.Clamp(percent, 0f, 1f);
		}

        public void SetState(UIShipEnergyCellState state)
		{
			fill.Value = 0f;

			SelfModulate = state switch
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