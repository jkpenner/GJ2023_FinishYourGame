using System;
using Godot;

namespace SpaceEngineer
{
    public partial class Weapon : Station
    {
        [Export] AmmoType ammoType;
        [Export] Interactable interactable;

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

            if (interactable is not null)
            {
                interactable.IsInteractable = true;
                interactable.Interacted += OnInteraction;
            }

            ItemPlaced += OnItemPlaced;
            ItemTaken += OnItemTaken;
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