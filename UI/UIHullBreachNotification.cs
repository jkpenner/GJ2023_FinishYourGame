using System;
using Godot;

namespace SpaceEngineer
{
    public partial class UIHullBreachNotification : Control
    {
        private const string TITLE_NODE_PATH = "MarginContainer/VBoxContainer/Title";
        private const string INFO_NODE_PATH = "MarginContainer/VBoxContainer/Information";

        private GameManager gameManager;
        private Label title;
        private Label info;

        private bool isFadingOut = false;

        public override void _Ready()
        {
            this.TryGetGameManager(out gameManager);
            FetchAndValidateSceneNodes();

            Hide();
        }

        public override void _EnterTree()
        {
            GameEvents.LifeSupportDepleting.Connect(OnLifeSupportDepleting);
            GameEvents.LifeSupportRestored.Connect(OnLifeSupportRestored);
        }

        public override void _ExitTree()
        {
            GameEvents.LifeSupportDepleting.Disconnect(OnLifeSupportDepleting);
            GameEvents.LifeSupportRestored.Disconnect(OnLifeSupportRestored);
        }

        public override void _Process(double delta)
        {
            if (gameManager.PlayerShip.IsLifeSupportDepleting)
            {
                info.Text = $"{(int)gameManager.PlayerShip.LifeSupportRemainingTime}s Remaining";
            }

            if (isFadingOut)
            {
                var color = Modulate;
                color.A = Mathf.Max(color.A - (float)delta, 0f);
                Modulate = color;

                if (color.A <= 0f)
                {
                    isFadingOut = false;
                    Hide();
                }
            }
        }

        private void FetchAndValidateSceneNodes()
        {
            title = GetNode<Label>(TITLE_NODE_PATH);
            if (title is null)
            {
                this.PrintMissingChildError(TITLE_NODE_PATH, nameof(Label));
            }

            info = GetNode<Label>(INFO_NODE_PATH);
            if (info is null)
            {
                this.PrintMissingChildError(INFO_NODE_PATH, nameof(Label));
            }
        }

        private void OnLifeSupportDepleting()
        {
            Show();
            Modulate = new Color("#FFFFFFFF");
            isFadingOut = false;

            title.Text = "Life Support Depleting";
        }

        private void OnLifeSupportRestored()
        {
            Show();
            Modulate = new Color("#FFFFFFFF");
            isFadingOut = true;

            title.Text = "Life Support Restore";
            info.Text = "All clear!";
        }
    }
}