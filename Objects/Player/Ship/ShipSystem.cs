using System;
using Godot;

namespace SpaceEngineer
{
    public class ShipSystem
    {
        public ShipSystemState State { get; private set; }
        public int NormalEnergy { get; private set; }
        public int OverclockEnergy { get; private set; }
        public float OverclockDuration { get; private set; }

        public int CurrentEnergyUsage => State switch
        {
            ShipSystemState.Powered => NormalEnergy,
            ShipSystemState.Damaged => NormalEnergy,
            ShipSystemState.Overclocked => OverclockEnergy,
            _ => 0
        };

        public event Action StateChanged;
        public event Action EnergyUsageChanged;

        private float overclockCounter;

        public ShipSystem(ShipSystemState state, int normalEnergy, int overclockEnergy, float overclockDuration)
        {
            State = state;
            NormalEnergy = normalEnergy;
            OverclockEnergy = overclockEnergy;
            OverclockDuration = overclockDuration;
        }

        public void Process(double delta)
        {
            if (State == ShipSystemState.Overclocked)
            {
                overclockCounter += (float)delta;
                if (overclockCounter >= OverclockDuration)
                {
                    // When a system comes out of the overclocked mode.
                    // It transitioned into the Damage state until repaired.
                    overclockCounter = 0f;
                    SetSystemState(ShipSystemState.Damaged);
                }
            }
        }

        protected void SetSystemState(ShipSystemState state)
        {
            if (State == state)
            {
                return;
            }

            var prevEnergy = CurrentEnergyUsage;

            State = state;
            StateChanged?.Invoke();

            if (prevEnergy != CurrentEnergyUsage)
            {
                EnergyUsageChanged?.Invoke();
            }
        }

        /// <summary>
        /// Repair a Damaged or Destroyed system.
        /// </summary>
        public void Repair()
        {
            // Since a damaged system is still considered online,
            // restore it to the online Powered state.
            if (State == ShipSystemState.Damaged)
            {
                SetSystemState(ShipSystemState.Powered);
            }
            // Since a destoryed system is still considered offline,
            // restore it to the offline Disabled state.
            else if (State == ShipSystemState.Destroyed)
            {
                SetSystemState(ShipSystemState.Disabled);
            }
            else
            {
                GD.Print("Attempting to repair a ship system that does not need to be repaired.");
            }
        }

        /// <summary>
        /// Toggle a system between the Powered or Disabled state.
        /// </summary>
        public void TogglePower()
        {
            if (State != ShipSystemState.Powered && State != ShipSystemState.Disabled)
            {
                return;
            }

            if (State == ShipSystemState.Powered)
            {
                SetSystemState(ShipSystemState.Disabled);
            }
            else
            {
                SetSystemState(ShipSystemState.Powered);
            }
        }

        /// <summary>
        /// Damage the ship system, if the system is already damaged
        /// then it is destroyed.
        /// </summary>
        public void Damage()
        {
            if (State == ShipSystemState.Destroyed)
            {
                return;
            }

            if (State == ShipSystemState.Damaged)
            {
                SetSystemState(ShipSystemState.Destroyed);
            }
            else
            {
                SetSystemState(ShipSystemState.Damaged);
            }
        }

        /// <summary>
        /// Destroy the ship system.
        /// </summary>
        public void Destroy()
        {
            if (State == ShipSystemState.Destroyed)
            {
                return;
            }

            SetSystemState(ShipSystemState.Destroyed);
        }

        /// <summary>
        /// Start overclocking the system.
        /// </summary>
        public void Overclock()
        {
            if (State == ShipSystemState.Powered || State == ShipSystemState.Disabled)
            {
                overclockCounter = 0f;
                SetSystemState(ShipSystemState.Overclocked);
            }
        }
    }
}