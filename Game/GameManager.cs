using System;
using Godot;

namespace SpaceEngineer
{
    public enum GameState
    {
        Initializing,
        Starting,
        Active,
        Paused,
        Victory,
        GameOver,
        Exiting,
    }

    public partial class GameManager : Node
    {
        [Export] ShipController playerShip;

        public GameState State { get; private set; }
        public bool IsGameActive => State == GameState.Active;

        public ShipController PlayerShip => playerShip;

        public GameManager()
        {
            State = GameState.Initializing;
        }

        public override void _Ready()
        {
            RegisterGlobalEvents();

            // Inform game that everything is setup and ready to go.
            SetGameState(GameState.Starting);
        }

        public override void _ExitTree()
        {
            UnregisterGlobalEvents();
        }

        private void SetGameState(GameState state)
        {
            if (State == state)
            {
                return;
            }

            GameEvents.GameStateExited.Emit(State);
            State = state;
            GD.Print($"Entering Game State: {State}");
            GameEvents.GameStateEntered.Emit(State);
            OnGameStateEntered();
        }

        private void OnGameStateEntered()
        {
            switch (State)
            {
                case GameState.Starting:
                    // Todo: Maybe do a count down or something in future, for now just start the game
                    SetGameState(GameState.Active);
                    break;
            }
        }

        public void TogglePause()
        {
            if (State == GameState.Active)
            {
                SetGameState(GameState.Paused);
            }
            else if (State == GameState.Paused)
            {
                SetGameState(GameState.Active);
            }
        }

        /// <summary>
        /// Trigger the game to display the game over state.
        /// </summary>
        public void TriggerGameOver()
        {
            SetGameState(GameState.GameOver);
        }

        /// <summary>
        /// Trigger the game to display the victory state.
        /// </summary>
        public void TriggerVictory()
        {
            SetGameState(GameState.Victory);
        }

        /// <summary>
        /// Will place the game in the exiting state. This should
        /// return the game back to the main menu.
        /// </summary>
        public void Exit()
        {
            SetGameState(GameState.Exiting);
        }

        private void RegisterGlobalEvents()
        {
            if (PlayerShip is not null)
            {
                PlayerShip.SystemStateChanged += GameEvents.ShipSystemStateChanged.Emit;
                PlayerShip.EnergyUsageChanged += GameEvents.ShipEnergyUsageChanged.Emit;
                PlayerShip.EnergyCapacityChanged += GameEvents.ShipEnergyCapacityChanged.Emit;

                PlayerShip.Overloading += GameEvents.ShipEnergyOverloaded.Emit;
                PlayerShip.OverloadEventStarted += GameEvents.ShipEnergyOverloaded.Emit;
                PlayerShip.EnergyNormalized += GameEvents.ShipEnergyNormalized.Emit;
            }
        }

        private void UnregisterGlobalEvents()
        {
            if (PlayerShip is not null)
            {
                PlayerShip.SystemStateChanged -= GameEvents.ShipSystemStateChanged.Emit;
                PlayerShip.EnergyUsageChanged -= GameEvents.ShipEnergyUsageChanged.Emit;
                PlayerShip.EnergyCapacityChanged -= GameEvents.ShipEnergyCapacityChanged.Emit;

                PlayerShip.Overloading -= GameEvents.ShipEnergyOverloaded.Emit;
                PlayerShip.OverloadEventStarted -= GameEvents.ShipEnergyOverloaded.Emit;
                PlayerShip.EnergyNormalized -= GameEvents.ShipEnergyNormalized.Emit;
            }
        }
    }
}