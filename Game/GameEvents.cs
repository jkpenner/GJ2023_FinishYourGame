namespace SpaceEngineer
{
    public static class GameEvents
    {
        /// <summary>
        /// Invoked when an inpact ship event occurs.
        /// </summary>
        public static readonly GameEvent Impact = new GameEvent();

        /// <summary>
        /// Occurs when the energy usage for all active system exceeds the ship's energy capacity
        /// and the ship is about to be overloaded.
        /// </summary>
        public static readonly GameEvent ShipEnergyOverloading = new GameEvent();
        
        /// <summary>
        /// Occurs when the energy usage for all active system exceeds the ship's energy capacity
        /// for an extended period of time.
        /// </summary>
        public static readonly GameEvent ShipEnergyOverloaded = new GameEvent();
        
        /// <summary>
        /// Occurs when the energy usage falls back below the ship's capacity after entering
        /// an overloading state.
        /// </summary>
        public static readonly GameEvent ShipEnergyNormalized = new GameEvent();

        /// <summary>
        /// Invoked when the amount of energy being used by the player ship changes.
        /// </summary>
        public static readonly GameEvent<int> ShipEnergyUsageChanged = new GameEvent<int>();

        /// <summary>
        /// Invoked when the total usable amount of energy for the player ship changes.
        /// </summary>
        public static readonly GameEvent<int> ShipEnergyCapacityChanged = new GameEvent<int>();

        public static readonly GameEvent<ShipSystemType> ShipSystemStateChanged = new GameEvent<ShipSystemType>();
        public static readonly GameEvent<ShipSystemType> SystemDestroyed = new GameEvent<ShipSystemType>();
    }
}