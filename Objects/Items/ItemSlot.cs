using System;
using Godot;

namespace SpaceEngineer
{
    public enum ItemSlotState
    {
        Moving,
        Idle,
    }

    public enum MoveMode
    {
        Instant,
        Slide,
    }

    public partial class ItemSlot : Area3D
    {
        [Export] Item initialItem;
        [Export] Node3D visualParent;

        public bool IsInteractable { get; set; }
        public Item HeldItem { get; private set; }

        public delegate bool ValidateItem(Item item);
        public ValidateItem ValidateItemHandler;

        private ItemSlotState state;
        private MoveMode moveMode;
        private Node3D itemVisual;

        public delegate void ItemEvent(Item item);
        public event ItemEvent ItemPlaced;
        public event ItemEvent ItemTaken;

        public bool CheckIfCanPlaceItem()
        {
            return HeldItem is null && IsInteractable;
        }

        public bool CheckIfCanTakeItem()
        {
            return state == ItemSlotState.Idle && HeldItem is not null && IsInteractable;
        }

        public bool TryPlaceObject(Item item)
        {
            if (item is null || HeldItem is not null)
            {
                return false;
            }

            HeldItem = item;
            itemVisual = HeldItem.InstantiateVisual();
            if (itemVisual is not null)
            {
                visualParent.AddChild(itemVisual);
                itemVisual.Position = Vector3.Zero;
                itemVisual.Rotation = Vector3.Zero;
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

            item = HeldItem;
            HeldItem = null;

            if (itemVisual is not null)
            {
                itemVisual.QueueFree();
            }

            return true;
        }

        public void MoveTo(ItemSlot other, MoveMode mode)
        {
            if (!CheckIfCanTakeItem() || !other.CheckIfCanPlaceItem())
            {
                return;
            }

            // Remove item from this object
            var item = this.HeldItem;
            var visual = this.itemVisual;

            this.HeldItem = null;
            this.itemVisual = null;

            state = ItemSlotState.Idle;
            moveMode = MoveMode.Instant;

            visualParent.RemoveChild(visual);

            // Add the item to the next slot
            other.HeldItem = item;
            other.itemVisual = visual;

            other.visualParent.AddChild(visual);
            visual.GlobalPosition = visualParent.GlobalPosition;

            other.state = ItemSlotState.Moving;
            other.moveMode = mode;
        }

        public override void _Process(double delta)
        {
            if (state == ItemSlotState.Moving && itemVisual is not null)
            {
                switch (moveMode)
                {
                    case MoveMode.Slide:
                        itemVisual.GlobalPosition = itemVisual.GlobalPosition.MoveToward(visualParent.GlobalPosition, (float)(delta));
                        if (itemVisual.GlobalPosition.DistanceTo(visualParent.GlobalPosition) < Mathf.Epsilon)
                        {
                            GD.Print("Move Complete");
                            state = ItemSlotState.Idle;
                        }
                        break;
                    case MoveMode.Instant:
                    
                        itemVisual.GlobalPosition = visualParent.GlobalPosition;
                        state = ItemSlotState.Idle;
                        break;
                }
            }
        }
    }
}