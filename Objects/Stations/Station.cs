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
        public const string ITEM_VISUAL_PARENT_NODE_PATH = "ItemVisualParent";

        public StationState State { get; private set; }
        public Item HeldItem { get; private set; }
        protected Node3D ItemVisual { get; private set; }

        private ItemMoveMode moveMode;
        private Node3D itemVisualParent;

        public delegate void ItemEvent(Item item);
        public event ItemEvent ItemPlaced;
        public event ItemEvent ItemTaken;

        public override void _Ready()
        {
            FetchAndValidateSceneNodes();
        }

        private void FetchAndValidateSceneNodes()
        {
            itemVisualParent = GetNode<Node3D>(ITEM_VISUAL_PARENT_NODE_PATH);
            if (itemVisualParent is null)
            {
                this.PrintMissingChildError(ITEM_VISUAL_PARENT_NODE_PATH, nameof(Node3D));
            }
        }

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
                itemVisualParent.AddChild(ItemVisual);
                ItemVisual.Position = Vector3.Zero;
                ItemVisual.Rotation = Vector3.Zero;
            }

            ItemPlaced?.Invoke(item);
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

            ItemTaken?.Invoke(item);
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

            if (visual is not null)
            {
                itemVisualParent.RemoveChild(visual);
            }

            // Add the item to the next slot
            other.HeldItem = item;
            other.ItemVisual = visual;

            if (visual is not null)
            {
                other.itemVisualParent.AddChild(visual);
                visual.GlobalPosition = itemVisualParent.GlobalPosition;
                visual.GlobalRotation = itemVisualParent.GlobalRotation;
            }

            other.State = StationState.MovingItem;
            other.moveMode = mode;

            this.ItemTaken?.Invoke(item);
            other.ItemPlaced?.Invoke(item);
        }

        public void DestroyItem()
        {
            HeldItem = null;
            if (ItemVisual is not null)
            {
                itemVisualParent.RemoveChild(ItemVisual);
                ItemVisual.QueueFree();
            }
            ItemVisual = null;
        }

        public void SpawnItem(Item item)
        {
            HeldItem = item;
            if (item is not null)
            {
                ItemVisual = item.InstantiateVisual();
                if (ItemVisual is not null)
                {
                    itemVisualParent.AddChild(ItemVisual);
                    ItemVisual.GlobalPosition = itemVisualParent.GlobalPosition;
                    ItemVisual.GlobalRotation = itemVisualParent.GlobalRotation;
                }
            }
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

                    ItemVisual.GlobalPosition = ItemVisual.GlobalPosition.MoveToward(itemVisualParent.GlobalPosition, (float)(delta));
                    if (ItemVisual.GlobalPosition.DistanceTo(itemVisualParent.GlobalPosition) < Mathf.Epsilon)
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

                    ItemVisual.GlobalPosition = itemVisualParent.GlobalPosition;
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