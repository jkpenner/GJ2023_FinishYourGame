using System;
using Godot;

namespace SpaceEngineer
{
    public partial class UIGameOverScreen : Control
    {
        private const string MAIN_MENU_SCENE = "res://Scenes/Main.tscn";
        private const string MAIN_MENU_BUTTON_PATH = "PanelContainer/MarginContainer/CenterContainer/VBoxContainer/HBoxContainer/MainMenu";
        private const string RETRY_BUTTON_PATH = "PanelContainer/MarginContainer/CenterContainer/VBoxContainer/HBoxContainer/Retry";

        private GameManager gameManager;
        private Button mainMenuButton;
        private Button retryButton;

        public override void _Ready()
        {
            this.TryGetGameManager(out gameManager);

            mainMenuButton = GetNode<Button>(MAIN_MENU_BUTTON_PATH);
            mainMenuButton.Pressed += OnMainMenuPressed;

            retryButton = GetNode<Button>(RETRY_BUTTON_PATH);
            retryButton.Pressed += OnRetryPressed;

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
            Visible = state == GameState.GameOver;
        }

        private void OnMainMenuPressed()
        {
            GetTree().ChangeSceneToFile(MAIN_MENU_SCENE);
        }

        private void OnRetryPressed()
        {
            GetTree().ReloadCurrentScene();
        }
    }
}