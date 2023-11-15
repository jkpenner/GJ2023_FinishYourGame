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
        public static readonly GameEvent<PlayerShip> ShipEnergyOverloading = new GameEvent<PlayerShip>();
        
        /// <summary>
        /// Occurs when the energy usage for all active system exceeds the ship's energy capacity
        /// for an extended period of time.
        /// </summary>
        public static readonly GameEvent<PlayerShip> ShipEnergyOverloaded = new GameEvent<PlayerShip>();
        
        /// <summary>
        /// Occurs when the energy usage falls back below the ship's capacity after entering
        /// an overloading state.
        /// </summary>
        public static readonly GameEvent<PlayerShip> ShipEnergyNormalized = new GameEvent<PlayerShip>();

        /// <summary>
        /// Invoked when the amount of energy being used by the player ship changes.
        /// </summary>
        public static readonly GameEvent<PlayerShip> ShipEnergyUsageChanged = new GameEvent<PlayerShip>();

        /// <summary>
        /// Invoked when the total usable amount of energy for the player ship changes.
        /// </summary>
        public static readonly GameEvent<PlayerShip> ShipEnergyCapacityChanged = new GameEvent<PlayerShip>();

        /// <summary>
        /// Occurs when a system is damaged.
        /// </summary>
        public static readonly GameEvent<ShipSystem> SystemDamaged = new GameEvent<ShipSystem>();

        /// <summary>
        /// Occurs when a system is destoyed.
        /// </summary>
        public static readonly GameEvent<ShipSystem> SystemDestroyed = new GameEvent<ShipSystem>();
    }
}