using System;
using Godot;

namespace SpaceEngineer
{
    public partial class UIInteractionPrompt : Control
    {

        private GameManager gameManager;
        private Interactable target;
        private TextureProgressBar progressBar;

        public override void _Ready()
        {
            this.TryGetGameManager(out gameManager);
            progressBar = GetNode<TextureProgressBar>("TextureProgressBar");
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
                    GD.Print(target.GetInteractPercent());
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
            target = interactable;
        }
    }
}