using Godot;
using SpaceEngineer;
using System;
using System.Threading;

public partial class UISystemStatus : Control
{

	private const string SYSTEM_NAME_NODE_PATH = "SystemPower/MarginContainer/HBoxContainer/SystemName";
	private const string POWER_CONTAINER_PATH = "SystemPower/MarginContainer/HBoxContainer/PanelContainer";
	private const string POWER_STATE_NODE_PATH = "SystemPower/MarginContainer/HBoxContainer/PanelContainer/MarginContainer/PowerState";
	private const string ENERGY_CONTAINER_PATH = "EnergyAmount";
	private const string ENERGY_AMOUNT_NODE_PATH = "EnergyAmount/MarginContainer/HBoxContainer/CenterContainer/Panel/EmergyAmount";
	private const string STATUS_PROGRESS_PATH = "Status/ProgressBar";
	private const string STATUS_LABEL_PATH = "Status/ProgressBar/Status";

	private const string SYSTEM_POWERED_ON_TEXT = "On";
	private const string SYSTEM_POWERED_OFF_TEXT = "Off";

	[Export] ShipSystemType systemType;
	[Export] Color poweredOn = new Color("#00FF00");
	[Export] Color poweredOff = new Color("#FF0000");
	[Export] Color damaged = new Color("#AA0000");
	[Export] Color overclocked = new Color("#ff5100");
	[Export] Color destroyed = new Color("#FF0000");

	private GameManager gameManager;

	private Label systemName;
	private Control powerContainer;
	private Label powerState;
	private Control energyContainer;
	private Label energyAmount;
	private ProgressBar statusProgress;
	private Label statusLabel;

	public override void _Ready()
	{
		this.TryGetGameManager(out gameManager);

		systemName = GetNode<Label>(SYSTEM_NAME_NODE_PATH);
		powerContainer = GetNode<Control>(POWER_CONTAINER_PATH);
		powerState = GetNode<Label>(POWER_STATE_NODE_PATH);
		energyContainer = GetNode<Control>(ENERGY_CONTAINER_PATH);
		energyAmount = GetNode<Label>(ENERGY_AMOUNT_NODE_PATH);
		statusProgress = GetNode<ProgressBar>(STATUS_PROGRESS_PATH);
		statusLabel = GetNode<Label>(STATUS_LABEL_PATH);

		systemName.Text = systemType.ToString();
		OnSystemStateChanged(systemType);
	}

	public override void _EnterTree()
	{
		GameEvents.ShipSystemStateChanged.Connect(OnSystemStateChanged);
	}

	public override void _ExitTree()
	{
		GameEvents.ShipSystemStateChanged.Connect(OnSystemStateChanged);
	}

	public override void _Process(double delta)
	{
		var system = gameManager.PlayerShip.GetSystem(systemType);
		if (system.State == ShipSystemState.Overclocked)
		{
			UpdateStatus(system);
		}
	}

	private void OnSystemStateChanged(ShipSystemType type)
	{
		if (systemType != type)
		{
			return;
		}

		var system = gameManager.PlayerShip.GetSystem(systemType);
		UpdatePowerState(system);
		UpdateEnergyAmount(system);
		UpdateStatus(system);
	}

	private void UpdatePowerState(ShipSystem system)
	{
		switch (system.State)
		{
			case ShipSystemState.Powered:
			case ShipSystemState.Damaged:
			case ShipSystemState.Overclocked:
				powerState.Text = SYSTEM_POWERED_ON_TEXT;
				powerContainer.SelfModulate = poweredOn;
				break;

			case ShipSystemState.Disabled:
			case ShipSystemState.Destroyed:
				powerState.Text = SYSTEM_POWERED_OFF_TEXT;
				powerContainer.SelfModulate = poweredOff;
				break;
		}
	}

	private void UpdateEnergyAmount(ShipSystem system)
	{
		switch (system.State)
		{
			case ShipSystemState.Powered:
			case ShipSystemState.Overclocked:
			case ShipSystemState.Damaged:
				energyContainer.Visible = true;
				energyAmount.Text = system.CurrentEnergyUsage.ToString();
				break;
			case ShipSystemState.Disabled:
			case ShipSystemState.Destroyed:
				energyContainer.Visible = false;
				break;
		}
	}

	private void UpdateStatus(ShipSystem system)
	{
		switch (system.State)
		{
			case ShipSystemState.Powered:
			case ShipSystemState.Disabled:
				statusProgress.Visible = false;
				break;
			case ShipSystemState.Damaged:
				statusProgress.Visible = true;
				statusProgress.Value = 1;
				statusProgress.SelfModulate = damaged;
				statusLabel.Text = system.State.ToString();
				break;
			case ShipSystemState.Destroyed:
				statusProgress.Visible = true;
				statusProgress.Value = 1;
				statusProgress.SelfModulate = destroyed;
				statusLabel.Text = system.State.ToString();
				break;
			case ShipSystemState.Overclocked:
				statusProgress.Visible = true;
				statusProgress.Value = system.OverclockRemainder / system.OverclockDuration;
				statusProgress.SelfModulate = overclocked;
				statusLabel.Text = system.State.ToString();
				break;
		}
	}
}
