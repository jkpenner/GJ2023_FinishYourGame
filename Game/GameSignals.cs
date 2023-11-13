using System;
using Godot;

namespace SpaceEngineer
{
    public partial class GameSignals : Node
    {
        [Signal] public delegate void ImpactEventHandler();

        public override void _EnterTree()
        {
            GameEvents.Impact.Connect(OnImpact);
        }

        public override void _ExitTree()
        {
            GameEvents.Impact.Disconnect(OnImpact);   
        }

        private void OnImpact()
        {
            EmitSignal(SignalName.Impact);
        }
    }
}