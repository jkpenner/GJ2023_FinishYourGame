using System;
using Godot;

namespace SpaceEngineer
{
    public partial class UIVictoryScreen : Control
    {
        private const string MAIN_MENU_BUTTON_PATH = "PanelContainer/MarginContainer/CenterContainer/VBoxContainer/HBoxContainer/MainMenu";
        private const string REPLAY_BUTTON_PATH = "PanelContainer/MarginContainer/CenterContainer/VBoxContainer/HBoxContainer/Replay";

        private GameManager gameManager;
        private Button mainMenuButton;
        private Button replayButton;

        public override void _Ready()
        {

            this.TryGetGameManager(out gameManager);

            mainMenuButton = GetNode<Button>(MAIN_MENU_BUTTON_PATH);
            mainMenuButton.Pressed += OnMainMenuPressed;

            replayButton = GetNode<Button>(REPLAY_BUTTON_PATH);
            replayButton.Pressed += OnReplayPressed;
            
            Visible = false;

            OnGameStateEntered(gameManager.State);
        }

        public override void _EnterTree()
        {
            GameEvents.GameStateEntered.Connect(OnGameStateEntered);
        }

        public override void _ExitTree()
        {
            GameEvents.GameStateEntered.Disconnect(OnGameStateEntered);
        }

        private void OnGameStateEntered(GameState state)
        {
            if (state == GameState.Victory)
            {
                Visible = true;
            }
        }

        private void OnMainMenuPressed()
        {
            gameManager.Exit();
        }

        private void OnReplayPressed()
        {
            gameManager.Restart();
        }
    }
}