using Godot;

namespace SpaceEngineer
{
    public partial class ShipSystemTerminalVisual : Node3D
    {
        [Export] ShaderMaterial normalShaderMaterial;
        [Export] ShaderMaterial progressShaderMaterial;
        [Export] Node3D overloadBody;

        private readonly static string[] NormalScreenPaths = new string[]
        {
            "MainMonitor",
            "SecondaryMonitor1",
            "SecondaryMonitor2",
        };

        private readonly static string[] ProgessScreenPaths = new string[]
        {
            "CenterIndicators",
        };

        private MeshInstance3D[] normalScreens;
        private MeshInstance3D[] progressScreens;

        public override void _Ready()
        {
            normalScreens = new MeshInstance3D[NormalScreenPaths.Length];
            overloadBody.Visible = false;

            for (int i = 0; i < NormalScreenPaths.Length; i++)
            {
                normalScreens[i] = GetNode<MeshInstance3D>(NormalScreenPaths[i]);
                normalScreens[i].SetSurfaceOverrideMaterial(0, normalShaderMaterial);
            }

            progressScreens = new MeshInstance3D[ProgessScreenPaths.Length];
            for (int i = 0; i < ProgessScreenPaths.Length; i++)
            {
                progressScreens[i] = GetNode<MeshInstance3D>(ProgessScreenPaths[i]);
                progressScreens[i].SetSurfaceOverrideMaterial(0, progressShaderMaterial);
            }
        }

        public void SetNormalScreenColor(Color color)
        {
                normalShaderMaterial.SetShaderParameter("fade", 0.0f);
                normalShaderMaterial.SetShaderParameter("main_color", color);
        }

        public void SetNormalScreenPulseMode(bool pulse, float pulseRate = 1f)
        {
                normalShaderMaterial.SetShaderParameter("pulse_main", pulse);
                normalShaderMaterial.SetShaderParameter("pulse_rate", pulseRate);
        }

        public void SetNormalScreenPulseColor(Color color)
        {
                normalShaderMaterial.SetShaderParameter("main_pulse_color", color);
        }

        public void SetProgressColors(Color mainColor, Color baseColor)
        {
                progressShaderMaterial.SetShaderParameter("main_color", mainColor);
                progressShaderMaterial.SetShaderParameter("base_color", baseColor);
        }

        public void SetProgressFade(float fade, float fadeSize = 0.2f)
        {
                progressShaderMaterial.SetShaderParameter("fade", fade);
                progressShaderMaterial.SetShaderParameter("fade_size", fadeSize);
        }

        public void SetProgressPulse(bool mainPulse, bool basePulse, float pulseRate = 1f)
        {
                progressShaderMaterial.SetShaderParameter("pulse_main", mainPulse);
                progressShaderMaterial.SetShaderParameter("pulse_base", basePulse);
                progressShaderMaterial.SetShaderParameter("pulse_rate", pulseRate);
        }

        public void SetProgressPulseColors(Color mainColor, Color baseColor)
        {
                progressShaderMaterial.SetShaderParameter("main_pulse_color", mainColor);
                progressShaderMaterial.SetShaderParameter("base_pulse_color", baseColor);
        }
        
        public void SetOverloadBodyVisible(bool visible)
        {
            overloadBody.Visible = visible;
        }
    }
}