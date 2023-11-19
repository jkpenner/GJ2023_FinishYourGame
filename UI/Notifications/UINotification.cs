using System;
using Godot;

namespace SpaceEngineer
{
    public partial class UINotification : Control
    {
        [Export] float fadeOutTime = 0.2f;

        public bool IsReadyForRemoval;
        public event Action ReadyForRemoval;

        private Label label;
        private bool isFadingOut;
        private float counter;

        public override void _Ready()
        {
            
        }

        public override void _Process(double delta)
        {
            counter -= (float)delta;

            if (isFadingOut)
            {
                var color = Modulate;
                color.A = Mathf.Clamp(counter / fadeOutTime, 0f, 1f);
                Modulate = color;

                if (color.A <= 0f)
                {
                    MarkForRemoval();
                }
            }
            else
            {                
                if (counter <= 0f)
                {
                    isFadingOut = true;
                    counter = fadeOutTime;
                }
            }
        }

        public void Setup(Notification notification)
        {
            label = GetNode<Label>("Label");
            label.Text = notification.Message;
            counter = notification.DisplayTime;
        }

        private void MarkForRemoval()
        {
            if (IsReadyForRemoval)
            {
                return;
            }

            Hide();
            IsReadyForRemoval = true;
            ReadyForRemoval?.Invoke();
        }
    }
}