using Godot;

namespace SpaceEngineer
{
    public partial class Interactable : Area3D
    {
        [Export] private bool isInteractable = true;
        [Export] private float interactDuration = 0f;
        [Export] Node3D promptLocation;

        private float counter = 0f;

        public delegate void InteractionEvent(PlayerController interactor);
        public event InteractionEvent Interacted;
        public event InteractionEvent InteractionStarted;
        public event InteractionEvent InteractionCanceled;

        public delegate bool ValidateInteractionEvent(PlayerController interator);
        public ValidateInteractionEvent ValidateInteraction;

        public bool ActiveInteraction { get; private set; }
        public PlayerController Interactor { get; private set; }

        public bool IsInstant => interactDuration <= 0f;

        public bool IsInteractable
        {
            get => isInteractable;
            set => isInteractable = value;
        }

        public Vector3 GetPromptPosition()
        {
            return promptLocation?.GlobalPosition ?? GlobalPosition;
        }

        public float GetInteractPercent()
        {
            return Mathf.Clamp(counter, 0f, interactDuration) / interactDuration;
        }

        public override void _Process(double delta)
        {
            if (!ActiveInteraction || Interactor is null)
            {
                return;
            }

            counter += (float)delta;
            if (counter >= interactDuration)
            {
                GD.Print($"Intracted with {GetParent()?.Name ?? Name}");
                Interacted?.Invoke(Interactor);

                ActiveInteraction = false;
                Interactor = null;
                counter = 0f;
            }
        }

        public bool CanInteract(PlayerController interactor)
        {
            return IsInteractable && (ValidateInteraction?.Invoke(interactor) ?? true);
        }

        public bool StartInteract(PlayerController interactor)
        {
            if (!IsInteractable || ActiveInteraction || interactor is null)
            {
                return false;
            }

            // If a validation method is assign check if interator is allowed to interact
            if (!CanInteract(interactor))
            {
                return false;
            }

            // Check if interaction is instant
            if (interactDuration <= 0f)
            {
                Interacted?.Invoke(interactor);
                return true;
            }

            ActiveInteraction = true;
            Interactor = interactor;
            counter = 0f;

            InteractionStarted?.Invoke(interactor);
            return true;
        }

        public void StopInteract(PlayerController interactor)
        {
            if (!ActiveInteraction || interactor != Interactor)
            {
                return;
            }

            ActiveInteraction = false;
            Interactor = null;
            counter = 0f;

            InteractionCanceled?.Invoke(interactor);
        }
    }
}