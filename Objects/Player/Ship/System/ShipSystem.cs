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

    public abstract partial class ShipSystem : Node3D
    {
        [Export] ShipSystemState initialState = ShipSystemState.Powered;
        [Export] int poweredEnergyUsage = 2;
        [Export] int overclockedEnergyUsage = 4;
        [Export] float overclockDuration = 30f;

        private float overclockCounter;

        public delegate void ShipSystemEvent(ShipSystem system);
        public event ShipSystemEvent StateChanged;

        public abstract ShipSystemType SystemType { get; }

        public PlayerShip Ship { get; set; }
        public ShipSystemState State { get; private set; }

        /// <summary>
        /// The amount of Power the system is currently requiring.
        /// </summary>
        public int EnergyUsage => State switch
        {
            ShipSystemState.Powered => poweredEnergyUsage,
            ShipSystemState.Damaged => poweredEnergyUsage,
            ShipSystemState.Overclocked => overclockedEnergyUsage,
            _ => 0
        };

        protected virtual void OnStateEntered(ShipSystemState state) { }
        protected virtual void OnStateExited(ShipSystemState state) { }

        public override void _Process(double delta)
        {
            if (State == ShipSystemState.Overclocked)
            {
                overclockCounter += (float)delta;
                if (overclockCounter >= overclockDuration)
                {
                    SetSystemState(ShipSystemState.Damaged);
                }
            }
        }

        public void Repair()
        {
            if (State == ShipSystemState.Damaged)
            {
                SetSystemState(ShipSystemState.Powered);
            }
            else if (State == ShipSystemState.Destroyed)
            {
                SetSystemState(ShipSystemState.Disabled);
            }
            else
            {
                GD.Print("Attempting to repair a ship system that does not need to be repaired.");
            }
        }

        public void TogglePower()
        {
            if (State == ShipSystemState.Powered)
            {
                SetSystemState(ShipSystemState.Disabled);
            }
            else
            {
                SetSystemState(ShipSystemState.Powered);
            }
        }

        public void Damage()
        {
            SetSystemState(ShipSystemState.Damaged);
            GameEvents.SystemDamaged.Emit(this);
        }

        public void Destroy()
        {
            SetSystemState(ShipSystemState.Destroyed);
            GameEvents.SystemDestroyed.Emit(this);
        }

        public void Overclock()
        {
            if (State == ShipSystemState.Powered || State == ShipSystemState.Disabled)
            {
                overclockCounter = 0f;
                SetSystemState(ShipSystemState.Overclocked);
            }
        }

        protected void SetSystemState(ShipSystemState state)
        {
            if (State == state)
            {
                return;
            }

            OnStateExited(State);
            State = state;
            GD.Print($"{SystemType} system entered {State} state.");
            OnStateEntered(State);

            StateChanged?.Invoke(this);
        }
    }
}