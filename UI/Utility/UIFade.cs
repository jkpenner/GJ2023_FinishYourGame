using System;
using Godot;

namespace SpaceEngineer
{
    public partial class UIFade : Control
    {
        private const string CLEAR_ANIM_NAME = "Clear";
        private const string FADE_OUT_ANIM_NAME = "FadeOut";
        private const string FADE_IN_ANIM_NAME = "FadeIn";

        [Export] bool fadeInOnStart = true;

        private AnimationPlayer animPlayer;

        public event Action FadeInCompleted;
        public event Action FadeOutCompleted;

        public override void _Ready()
        {
            animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
            animPlayer.AnimationFinished += OnFadeFinished;

            if (fadeInOnStart)
            {
                FadeIn();
            }
            else
            {
                Clear();
            }
        }

        public void Clear()
        {
            animPlayer.Play(CLEAR_ANIM_NAME);
        }

        public void FadeIn()
        {
            animPlayer.Play(FADE_IN_ANIM_NAME);
        }

        public void FadeOut()
        {
            animPlayer.Play(FADE_OUT_ANIM_NAME);
        }

        private void OnFadeFinished(StringName animName)
        {
            if (animName == FADE_OUT_ANIM_NAME)
            {
                FadeOutCompleted?.Invoke();
            }
            else if (animName == FADE_IN_ANIM_NAME)
            {
                FadeInCompleted?.Invoke();
            }
        }
    }
}