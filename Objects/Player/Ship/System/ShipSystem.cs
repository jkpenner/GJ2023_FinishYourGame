using Godot;

namespace SpaceEngineer
{
    public enum SystemState
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

    public partial class ShipSystem : Node3D
    {
        [Export] SystemState initialState;
        [Export] int poweredEnergyUsage;
        [Export] int overclockedEnergyUsage;

        public delegate void ShipSystemEvent(ShipSystem system);
        public event ShipSystemEvent StateChanged;

        public PlayerShip Ship { get; set; }
        public SystemState State { get; private set; }

        /// <summary>
        /// The amount of Power the system is currently requiring.
        /// </summary>
        public int PowerUsage => State switch
        {
            SystemState.Powered | SystemState.Damaged => poweredEnergyUsage,
            SystemState.Overclocked => overclockedEnergyUsage,
            _ => 0
        };

        protected virtual void OnStateEntered(SystemState state) {}
        protected virtual void OnStateExited(SystemState state) {}

        public void Damage()
        {
            SetSystemState(SystemState.Damaged);
        }

        public void Destroy()
        {
            SetSystemState(SystemState.Destroyed);
        }

        protected void SetSystemState(SystemState state)
        {
            if (State == state)
            {
                return;
            }

            OnStateExited(State);
            State = state;
            OnStateEntered(State);

            StateChanged?.Invoke(this);
        }
    }
}