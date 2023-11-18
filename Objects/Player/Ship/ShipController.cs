using System;
using System.Linq;
using Godot;

namespace SpaceEngineer
{
    public enum ShipSystemType
    {
        Engines,
        Weapons,
        Shields,
        Sensors,
    }

    public enum ShipSystemState
    {
        /// <summary>
        /// System is powered off. It will not consume energy and is not functioning.
        /// </summary>
        Disabled,
        /// <summary>
        /// System is powered on. It is functioning at normal capacity.
        /// </summary>
        Powered,
        /// <summary>
        /// System is powered on. It is functioning at an increased capacity.
        /// </summary>
        Overclocked,
        /// <summary>
        /// System is powered on. It is functioning at a reduced capacity and
        /// must be repaired to return to its normal state.
        /// </summary>
        Damaged,
        /// <summary>
        /// System is powered off. It does not consume energy and is not functioning.
        /// It must be repaired in order to be powered back on.
        /// </summary>
        Destroyed,
    }

    public enum ShipOverloadState
    {
        NotOverloaded,
        Overloading,
        Overloaded,
    }

    public partial class ShipController : Node3D
    {
        [Export] int initialEnergyCapacity = 10;
        [Export] int maximumEnergy = 10;
        [Export] float energyRegenDuration = 30f;
        // A value of 1 would increase the counter by 1 every second, which
        // would essentially reduce the recharge rate by half.
        [Export] float energyRegenPerPlayerRate = 1f;
        [Export] float energyRegenMaxPlayerRate = 2f; // Multiplayer?
        [Export] float timeTillOverload = 15f;

        [ExportGroup("Weapon System")]
        [Export] ShipSystemState initialWeaponState = ShipSystemState.Powered;
        [Export] int weaponNormalEnergy = 2;
        [Export] int weaponOverclockEnergy = 4;
        [Export] float weaponOverclockDuration = 30f;

        [ExportGroup("Engine System")]
        [Export] ShipSystemState initialEngineState = ShipSystemState.Powered;
        [Export] int engineNormalEnergy = 2;
        [Export] int engineOverclockEnergy = 4;
        [Export] float engineOverclockDuration = 30f;

        [ExportGroup("Shield System")]
        [Export] ShipSystemState initialShieldState = ShipSystemState.Powered;
        [Export] int shieldNormalEnergy = 2;
        [Export] int shieldOverclockEnergy = 4;
        [Export] float shieldOverclockDuration = 30f;

        [ExportGroup("Sensor System")]
        [Export] ShipSystemState initialSensorState = ShipSystemState.Powered;
        [Export] int sensorNormalEnergy = 2;
        [Export] int sensorOverclockEnergy = 4;
        [Export] float sensorOverclockDuration = 30f;

        [ExportGroup("In Scene References")]
        [Export] Godot.Collections.Array<Weapon> weapons;
        [Export] Godot.Collections.Array<DamagableHull> hulls;
        [Export] Godot.Collections.Array<Treadmill> treadmills;

        public ShipSystem WeaponSystem { get; private set; }
        public ShipSystem EngineSystem { get; private set; }
        public ShipSystem ShieldSystem { get; private set; }
        public ShipSystem SensorSystem { get; private set; }

        public int MaximumEnergy => maximumEnergy;
        public int EnergyCapacity { get; private set; }
        public int EnergyUsage { get; private set; }
        public ShipOverloadState OverloadState { get; private set; }

        private float overloadCounter;
        private float energyRegenCounter;
        private int energyRegenPlayerInput;

        public event Action Overloading;
        public event Action OverloadEventStarted;
        public event Action OverloadEventCompleted;
        public event Action EnergyNormalized;

        public event Action<int> EnergyUsageChanged;
        public event Action<int> EnergyCapacityChanged;
        public event Action<ShipSystemType> SystemStateChanged;

        public ShipController()
        {
            WeaponSystem = new ShipSystem();
            EngineSystem = new ShipSystem();
            ShieldSystem = new ShipSystem();
            SensorSystem = new ShipSystem();
        }

        public override void _Ready()
        {
            EnergyCapacity = initialEnergyCapacity;

            WeaponSystem.Setup(initialWeaponState, weaponNormalEnergy, weaponOverclockEnergy, weaponOverclockDuration);
            EngineSystem.Setup(initialEngineState, engineNormalEnergy, engineOverclockEnergy, engineOverclockDuration);
            ShieldSystem.Setup(initialShieldState, shieldNormalEnergy, shieldOverclockEnergy, shieldOverclockDuration);
            SensorSystem.Setup(initialSensorState, sensorNormalEnergy, sensorOverclockEnergy, sensorOverclockDuration);

            WeaponSystem.EnergyUsageChanged += OnEnergyUsageChanged;
            EngineSystem.EnergyUsageChanged += OnEnergyUsageChanged;
            ShieldSystem.EnergyUsageChanged += OnEnergyUsageChanged;
            SensorSystem.EnergyUsageChanged += OnEnergyUsageChanged;

            WeaponSystem.StateChanged += () => SystemStateChanged?.Invoke(ShipSystemType.Weapons);
            ShieldSystem.StateChanged += () => SystemStateChanged?.Invoke(ShipSystemType.Shields);
            EngineSystem.StateChanged += () => SystemStateChanged?.Invoke(ShipSystemType.Engines);
            SensorSystem.StateChanged += () => SystemStateChanged?.Invoke(ShipSystemType.Sensors);

            foreach (var treadmill in treadmills)
            {
                treadmill.EnergyGenerated += IncrementEnergyRegen;
            }

            // Initial calls to populate values
            OnEnergyUsageChanged();
        }

        public override void _Process(double delta)
        {
            WeaponSystem.Process(delta);
            EngineSystem.Process(delta);
            ShieldSystem.Process(delta);
            SensorSystem.Process(delta);

            if (OverloadState == ShipOverloadState.Overloading)
            {
                overloadCounter += (float)delta;
                if (overloadCounter >= timeTillOverload)
                {
                    OverloadState = ShipOverloadState.Overloaded;
                    OnOverloadEvent();
                }
            }

            if (EnergyCapacity < MaximumEnergy)
            {
                energyRegenCounter += GetEnergyRegenRate() * (float)delta;

                if (energyRegenCounter > energyRegenDuration)
                {
                    var newEnergyCapacity = Mathf.Min(EnergyCapacity + 1, MaximumEnergy);
                    if (EnergyCapacity != newEnergyCapacity)
                    {
                        energyRegenCounter = 0f;
                        EnergyCapacity = newEnergyCapacity;
                        EnergyCapacityChanged?.Invoke(EnergyCapacity);
                        OnEnergyUsageChanged();
                    }
                }
            }

            // Reset amount of player input each frame.
            energyRegenPlayerInput = 0;
        }

        /// <summary>
        /// Inform the ship that a player is generating energy.
        /// Method must be called each frame.
        /// </summary>
        public void IncrementEnergyRegen()
        {
            energyRegenPlayerInput += 1;
        }

        public float GetEnergyRegenRate()
        {
            if (EnergyCapacity >= MaximumEnergy)
            {
                return 0f;
            }

            return 1f + Mathf.Min(energyRegenPerPlayerRate * energyRegenPlayerInput, energyRegenMaxPlayerRate);
        }

        public ShipSystem GetSystem(ShipSystemType systemType)
        {
            return systemType switch
            {
                ShipSystemType.Engines => EngineSystem,
                ShipSystemType.Weapons => WeaponSystem,
                ShipSystemType.Shields => ShieldSystem,
                ShipSystemType.Sensors => SensorSystem,
                _ => throw new NotImplementedException()
            };
        }

        public float GetRemainingTimeTillOverload()
        {
            return Mathf.Max(timeTillOverload - overloadCounter, 0f);
        }

        private void OnEnergyUsageChanged()
        {
            EnergyUsage = WeaponSystem.CurrentEnergyUsage
                + EngineSystem.CurrentEnergyUsage
                + ShieldSystem.CurrentEnergyUsage
                + SensorSystem.CurrentEnergyUsage;

            EnergyUsageChanged?.Invoke(EnergyUsage);

            if (EnergyUsage > EnergyCapacity && OverloadState == ShipOverloadState.NotOverloaded)
            {
                GD.Print($"Ship energy is overloading ({timeTillOverload} seconds)");
                OverloadState = ShipOverloadState.Overloading;
                overloadCounter = 0f;

                Overloading?.Invoke();
            }
            else if (EnergyUsage <= EnergyCapacity && OverloadState == ShipOverloadState.Overloading)
            {
                GD.Print("Ship energy returned to normal");
                OverloadState = ShipOverloadState.NotOverloaded;
                overloadCounter = 0f;

                EnergyNormalized?.Invoke();
            }
        }

        private void OnOverloadEvent()
        {
            OverloadEventStarted?.Invoke();

            DestroyAllOverclockedSystems();
            DestroyRandomUntilNormalized();

            // Stop overload event and reduce energy capacity by 1
            OverloadState = ShipOverloadState.NotOverloaded;

            var newEnergyCapacity = Mathf.Max(EnergyCapacity - 1, 0);
            if (EnergyCapacity != newEnergyCapacity)
            {
                EnergyCapacity = newEnergyCapacity;
                EnergyCapacityChanged?.Invoke(EnergyCapacity);
            }

            OverloadEventCompleted?.Invoke();
        }

        private void DestroyAllOverclockedSystems()
        {
            foreach (var systemType in GetSystemTypeArray())
            {
                var system = GetSystem(systemType);
                if (system.State != ShipSystemState.Overclocked)
                {
                    continue;
                }

                system.Destroy();
            }
        }

        private void DestroyRandomUntilNormalized()
        {
            if (EnergyUsage <= EnergyCapacity)
            {
                return;
            }

            Random random = new Random();
            var systemTypes = GetSystemTypeArray().ToList();

            for (int i = systemTypes.Count; i > 0; i--)
            {
                int index = random.Next(i);
                var systemType = systemTypes[index];
                systemTypes.RemoveAt(index);

                var system = GetSystem(systemType);
                if (system.State != ShipSystemState.Powered)
                {
                    system.TogglePower();
                }
                else if (system.State != ShipSystemState.Damaged)
                {
                    system.Destroy();
                }

                if (EnergyUsage <= EnergyCapacity)
                {
                    break;
                }
            }
        }

        private ShipSystemType[] GetSystemTypeArray()
        {
            return new ShipSystemType[] {
                ShipSystemType.Engines,
                ShipSystemType.Shields,
                ShipSystemType.Weapons,
                ShipSystemType.Sensors,
            };
        }
    }
}