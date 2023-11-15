using Godot;

namespace SpaceEngineer
{
    public partial class GameManager : Node
    {
        [Export] PlayerShip playerShip;


        public PlayerShip PlayerShip => playerShip;
    }
}