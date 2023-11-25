using System;
using Godot;

namespace SpaceEngineer
{
    public enum ConverterState
    {
        Idle,
        Loading,
        Processing,
        Unloading,
        Reset
    }

    public partial class Converter : Station
    {
        public override string ITEM_VISUAL_PARENT_NODE_PATH { get => "ConstructionTerminal/ConstructionTerminalBones/Skeleton3D/BoneAttachment3D/ItemVisualParent"; }

        [Export] private Item inputItem;
        [Export] private Item outputItem;
        [Export] private Station outputTarget;
        [Export] private float processTime = 5.0f;

        private float counter = 0.0f;
        private bool isProcessed = false;
        private ConverterState converterState = ConverterState.Idle;
        private AnimationPlayer animationPlayer;
        private MeshInstance3D display;

        public override void _Ready()
        {
            base._Ready();

            converterState = ConverterState.Idle;
            animationPlayer = GetNode<AnimationPlayer>("ConstructionTerminal/AnimationPlayer");
            animationPlayer.Play("TerminalIdle");
            animationPlayer.AnimationFinished += OnAnimationFinished;

            display = GetNode<MeshInstance3D>("ConstructionTerminal/ConstructionTerminalBones/ConstructionTerminal2/MonitorScreen");
        }

        private void OnAnimationFinished(StringName animName)
        {
            if (animName == "TerminalLoad")
            {
                converterState = ConverterState.Processing;
            }
            else if (animName == "TerminalUnload")
            {
                MoveTo(outputTarget, ItemMoveMode.Slide);
                converterState = ConverterState.Reset;
                animationPlayer.Play("TerminalReset");
            }
            else if (animName == "TerminalReset")
            {
                converterState = ConverterState.Idle;

            }
        }

        public override bool AllowItemChange()
        {
            if (HeldItem is not null && converterState == ConverterState.Unloading)
            {
                return true;
            }

            return converterState == ConverterState.Idle;
        }

        public override bool ValidateItem(Item item)
        {
            if (inputItem is null)
            {
                return true;
            }

            return inputItem == item;
        }

        public override void _Process(double delta)
        {
            base._Process(delta);

            SetDisplayActive(converterState == ConverterState.Processing);

            if (State == StationState.Idle)
            {
                switch (converterState)
                {
                    case ConverterState.Idle:
                        if (HeldItem is not null)
                        {
                            converterState = ConverterState.Loading;
                            animationPlayer.Play("TerminalLoad");
                        }
                        break;
                    case ConverterState.Processing:
                        counter += (float)delta;
                        if (counter >= processTime && !isProcessed)
                        {
                            isProcessed = true;

                            counter = 0f;

                            // Switch out the old item for the new
                            DestroyItem();
                            SpawnItem(outputItem);
                        }

                        if (isProcessed && outputTarget.HeldItem is null)
                        {
                            converterState = ConverterState.Unloading;
                            animationPlayer.Play("TerminalUnload");
                            isProcessed = false;
                        }
                        break;
                }
            }
        }

        private void SetDisplayActive(bool isActive)
        {
            var material = display.GetSurfaceOverrideMaterial(0);
            if (material is ShaderMaterial shader)
            {
                shader.SetShaderParameter("pulse_main", isActive);
            }
        }
    }
}