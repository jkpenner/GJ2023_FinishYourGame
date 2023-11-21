using System;
using Godot;

namespace SpaceEngineer
{
    public partial class UIInteractionPrompt : Control
    {
        private const string ACTION_NAME_PANEL_PATH = "ActionNamePanel";
        private const string ACTION_NAME_LABEL_PATH = "ActionNamePanel/MarginContainer/ActionName";


        private GameManager gameManager;
        private Interactable target;
        private TextureProgressBar progressBar;

        private Control actionNameParent;
        private Label actionName;

        public override void _Ready()
        {
            this.TryGetGameManager(out gameManager);
            progressBar = GetNode<TextureProgressBar>("TextureProgressBar");

            actionNameParent = GetNode<Control>(ACTION_NAME_PANEL_PATH);
            if (actionNameParent is null)
            {
                this.PrintMissingChildError(ACTION_NAME_PANEL_PATH, nameof(Control));
            }

            actionName = GetNode<Label>(ACTION_NAME_LABEL_PATH);
            if (actionName is null)
            {
                this.PrintMissingChildError(ACTION_NAME_LABEL_PATH, nameof(Label));
            }
        }

        public override void _EnterTree()
        {
            GameEvents.PlayerTargetChanged.Connect(OnPlayerTargetChanged);
        }

        public override void _ExitTree()
        {
            GameEvents.PlayerTargetChanged.Disconnect(OnPlayerTargetChanged);
        }

        public override void _Process(double delta)
        {
            if (target is not null && (target.CanInteract(gameManager.Player) || target.ActiveInteraction))
            {
                Show();
                var worldPosition = target.GetPromptPosition();
                GlobalPosition = GetViewport().GetCamera3D().UnprojectPosition(worldPosition);

                if (target.IsInstant)
                {
                    progressBar.Value = 0f;
                }
                else
                {
                    progressBar.Value = target.GetInteractPercent();
                }
            }
            else
            {
                Hide();
                progressBar.Value = 0f;
            }
        }

        private void OnPlayerTargetChanged(Interactable interactable)
        {
            if (target is not null)
            {
                target.ActionTextChanged -= SetActionText;
            }

            target = interactable;
            if (target is not null)
            {
                SetActionText(target.ActionText);
                target.ActionTextChanged += SetActionText;
            }
        }

        private void SetActionText(string actionText)
        {
            actionName.Text = actionText;
            actionNameParent.Visible = !string.IsNullOrEmpty(actionText);
        }

    }
}