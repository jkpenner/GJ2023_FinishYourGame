using System;
using Godot;

namespace SpaceEngineer
{
    public partial class Weapon : Station
    {
        public const string INTERACTABLE_NODE_PATH = "Interactable";

        [Export] AmmoType ammoType;
        [Export] string displayName;

        private ShipController ship;
        private Interactable interactable;

        public AmmoType AmmoType => ammoType;
        public string DisplayName => displayName;

        public delegate void WeaponEvent(Weapon weapon);

        /// <summary>
        /// Invoked when the correct ammo is loaded into the weapon
        /// and it is ready to be fired.
        /// </summary>
        public event WeaponEvent WeaponArmed;

        /// <summary>
        /// Invoked when the weapon's ammo is removed and is no longer
        /// ready to be fired.
        /// </summary>
        public event WeaponEvent WeaponDisarmed;

        public override void _Ready()
        {
            base._Ready();

            FetchAndValidateSceneNodes();

            ship = this.FindParentOfType<ShipController>();
            if (ship is null)
            {
                GD.PrintErr($"[{nameof(DamagableHull)}]: Node must be a child of a {nameof(ShipController)} node.");
                QueueFree();
                return;
            }

            ship.RegisterWeapon(this);

            ItemPlaced += OnItemPlaced;
            ItemTaken += OnItemTaken;
        }

        private void FetchAndValidateSceneNodes()
        {
            interactable = GetNode<Interactable>("Interactable");
            if (interactable is not null)
            {
                interactable.IsInteractable = true;
                interactable.ValidateInteraction = OnValidateInteration;
                interactable.Interacted += OnInteraction;
            }
            else
            {
                this.PrintMissingChildError(INTERACTABLE_NODE_PATH, nameof(Interactable));
            }
        }

        private bool OnValidateInteration(PlayerController interactor)
        {
            if (HeldItem is null && interactor.HeldItem is not null && interactor.HeldItem.AmmoType == ammoType)
            {
                interactable.SetActionText("Load Ammo");
                return true;
            }
            else if (HeldItem is not null && interactor.HeldItem is null)
            {
                interactable.SetActionText("Take Ammo");
                return true;
            }
            return false;
        }

        public override void _ExitTree()
        {
            if (ship is not null)
            {
                ship.UnregisterWeapon(this);
            }
        }

        private void OnItemPlaced(Item item)
        {
            WeaponArmed?.Invoke(this);
        }

        private void OnItemTaken(Item item)
        {
            WeaponDisarmed?.Invoke(this);
        }

        public override bool ValidateItem(Item item)
        {
            return item.AmmoType == ammoType;
        }

        private void OnInteraction(PlayerController interactor)
        {
            if (interactor.HeldItem is not null)
            {
                if (TryPlaceObject(interactor.HeldItem))
                {
                    interactor.SetHeldItem(null);
                }
                else
                {
                    GD.Print("Failed to place item");
                }
            }
            else
            {
                if (TryTakeObject(out var item))
                {
                    interactor.SetHeldItem(item);
                }
                else
                {
                    GD.Print("Failed to take item");
                }
            }
        }

        public bool IsReadyToFire()
        {
            return HeldItem is not null && HeldItem.AmmoType == ammoType;
        }

        public void StartFiringProceedure()
        {
            if (interactable is not null)
            {
                interactable.IsInteractable = false;
            }

            // Play charging the weapon animation and effect here
        }

        public void CancelFiring()
        {
            if (interactable is not null)
            {
                interactable.IsInteractable = true;
            }

            // Play cancel animation and effects here
        }

        public void Fire()
        {
            if (interactable is not null)
            {
                interactable.IsInteractable = true;
            }

            // Play firing animation and effects here
        }
    }
}