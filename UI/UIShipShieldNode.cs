using System;
using Godot;

namespace SpaceEngineer
{
    public partial class UIShipShieldNode : Control
    {
        private TextureProgressBar progress;

        public override void _Ready()
        {
            progress = GetNode<TextureProgressBar>("Progress");
        }

        public void SetValue(float value)
        {
            progress.Value = value;
        }
    }
}