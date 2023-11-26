using System.Collections.Generic;
using Godot;

namespace SpaceEngineer
{
    public partial class UIHullBreachWarnings : Control
    {
        private GameManager gameManager;
        private Dictionary<DamagableHull, TextureRect> hullMap = new Dictionary<DamagableHull, TextureRect>();

        public override void _Ready()
        {
            this.TryGetGameManager(out gameManager);
            foreach (var hull in gameManager.PlayerShip.Hulls)
            {
                hull.HullDamaged += OnHullDamaged;
                hull.HullRepaired += OnHullRepaired;
            }
        }

        public override void _ExitTree()
        {
            foreach (var hull in gameManager.PlayerShip.Hulls)
            {
                hull.HullDamaged -= OnHullDamaged;
                hull.HullRepaired -= OnHullRepaired;
            }
        }

        public override void _Process(double delta)
        {
            var rect = GetViewportRect();
            rect = rect.Grow(-rect.Size.X * 0.1f);

            var camera = GetViewport().GetCamera3D();

            foreach (var (hull, control) in hullMap)
            {
                var screenPosition = camera.UnprojectPosition(hull.GlobalWarningPosition);


                if (rect.HasPoint(screenPosition) && camera.IsPositionInFrustum(hull.GlobalWarningPosition) && !camera.IsPositionBehind(hull.GlobalPosition))
                {
                    control.GlobalPosition = screenPosition - (control.Size / 2f);
                }
                else
                {
                    Vector3 playerDirection = Vector3.Back; // Game forward is negative z
                    Vector3 hullDirection = (hull.GlobalWarningPosition - gameManager.Player.GlobalPosition).Normalized();
                    float angle = playerDirection.SignedAngleTo(hullDirection, Vector3.Up);

                    Vector2 screenSize = GetViewport().GetVisibleRect().Size;
                    Vector2 screenCenter = screenSize / 2;
                    float radius = 200f;

                    Vector2 iconPosition = screenCenter + new Vector2(Mathf.Sin(angle), Mathf.Cos(angle)) * radius;
                    control.GlobalPosition = iconPosition - (control.Size / 2f);
                }


            }
        }


        private void OnHullDamaged(DamagableHull hull)
        {
            if (!hullMap.ContainsKey(hull))
            {
                var control = new TextureRect();
                control.ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize;
                control.StretchMode = TextureRect.StretchModeEnum.Scale;
                control.Size = new Vector2(52, 52);
                control.MouseFilter = MouseFilterEnum.Ignore;
                control.Texture = ResourceLoader.Load<Texture2D>("res://Assets/Textures/WarningSign.png");

                AddChild(control);
                hullMap.Add(hull, control);
            }

            if (hull.State == HullState.Damaged)
            {
                hullMap[hull].SelfModulate = new Color("#CCCC00");
            }
            else if (hull.State == HullState.Breached)
            {
                hullMap[hull].SelfModulate = new Color("#CC3300");
            }
        }


        private void OnHullRepaired(DamagableHull hull)
        {
            if (hull.State == HullState.Damaged)
            {
                hullMap[hull].SelfModulate = new Color("#CCCC00");
            }
            else if (hull.State == HullState.Armored)
            {
                var control = hullMap[hull];
                hullMap.Remove(hull);
                RemoveChild(control);
                control.QueueFree();
            }
        }
    }
}