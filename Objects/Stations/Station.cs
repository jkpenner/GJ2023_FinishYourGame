using System;
using Godot;

namespace SpaceEngineer
{
    public enum StationState
    {
        MovingItem,
        Idle,
    }

    public enum ItemMoveMode
    {
        Instant,
        Slide,
    }

    public partial class Station : Node3D
    {
        [Export] Node3D visualParent;

        public StationState State { get; private set; }
        public Item HeldItem { get; private set; }
        protected Node3D ItemVisual { get; private set; }

        private ItemMoveMode moveMode;

        public delegate void ItemEvent(Item item);
        public event ItemEvent ItemPlaced;
        public event ItemEvent ItemTaken;

        /// <summary>
        /// Check if the station allows the held item to be taken or replaced.
        /// </summary>
        public virtual bool AllowItemChange()
        {
            return true;
        }

        /// <summary>
        /// Validation method to check if an item can be placed on the station.
        /// </summary>
        public virtual bool ValidateItem(Item item)
        {
            // Will accept any item
            return true;
        }

        public bool CheckIfCanPlaceItem()
        {
            return HeldItem is null && AllowItemChange();
        }

        public bool CheckIfCanTakeItem()
        {
            if (State == StationState.MovingItem)
            {
                return false;
            }

            return HeldItem is not null && AllowItemChange();
        }

        public bool TryPlaceObject(Item item)
        {
            if (item is null || HeldItem is not null)
            {
                return false;
            }

            if (!ValidateItem(item))
            {
                return false;
            }

            HeldItem = item;
            ItemVisual = HeldItem.InstantiateVisual();
            if (ItemVisual is not null)
            {
                visualParent.AddChild(ItemVisual);
                ItemVisual.Position = Vector3.Zero;
                ItemVisual.Rotation = Vector3.Zero;
            }
            return true;
        }

        public bool TryTakeObject(out Item item)
        {
            if (HeldItem is null)
            {
                item = null;
                return false;
            }

            if (State == StationState.MovingItem)
            {
                CompleteItemMove();
            }

            item = HeldItem;
            HeldItem = null;

            if (ItemVisual is not null)
            {
                ItemVisual.QueueFree();
            }

            return true;
        }

        public void MoveTo(Station other, ItemMoveMode mode)
        {
            if (!CheckIfCanTakeItem() || !other.CheckIfCanPlaceItem())
            {
                return;
            }

            if (!other.ValidateItem(this.HeldItem))
            {
                return;
            }

            // Remove item from this object
            var item = this.HeldItem;
            var visual = this.ItemVisual;

            this.HeldItem = null;
            this.ItemVisual = null;

            State = StationState.Idle;
            moveMode = ItemMoveMode.Instant;

            visualParent.RemoveChild(visual);

            // Add the item to the next slot
            other.HeldItem = item;
            other.ItemVisual = visual;

            other.visualParent.AddChild(visual);
            visual.GlobalPosition = visualParent.GlobalPosition;

            other.State = StationState.MovingItem;
            other.moveMode = mode;
        }

        public override void _Process(double delta)
        {
            if (State == StationState.MovingItem)
            {
                OnItemMoveUpdate(delta, moveMode);
            }
        }

        /// <summary>
        /// Method called with an Item Move is started.
        /// </summary>
        protected virtual void OnItemMoveStart() { }

        /// <summary>
        /// Method called with an Item Move completes.
        /// </summary>
        protected virtual void OnItemMoveComplete() { }

        /// <summary>
        /// Method called when an Item Move is updated. Override for a custom
        /// movement. Must call <code>CompleteItemMove</code> at end of movement.
        /// </summary>
        protected virtual void OnItemMoveUpdate(double delta, ItemMoveMode mode)
        {
            switch (mode)
            {
                case ItemMoveMode.Slide:
                    if (ItemVisual is null)
                    {
                        CompleteItemMove();
                        return;
                    }

                    ItemVisual.GlobalPosition = ItemVisual.GlobalPosition.MoveToward(visualParent.GlobalPosition, (float)(delta));
                    if (ItemVisual.GlobalPosition.DistanceTo(visualParent.GlobalPosition) < Mathf.Epsilon)
                    {
                        CompleteItemMove();
                    }
                    break;
                case ItemMoveMode.Instant:
                    if (ItemVisual is null)
                    {
                        CompleteItemMove();
                        return;
                    }

                    ItemVisual.GlobalPosition = visualParent.GlobalPosition;
                    CompleteItemMove();
                    break;
            }
        }

        /// <summary>
        /// Inform the station that an Item Movement is completed.
        /// </summary>
        protected void CompleteItemMove()
        {
            State = StationState.Idle;
            OnItemMoveComplete();
        }
    }
}