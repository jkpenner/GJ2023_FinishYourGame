using System;
using System.Collections.Generic;
using Godot;

namespace SpaceEngineer
{
    public partial class UIGameNotifications : Control
    {
        [Export] PackedScene notificationScene;
        [Export] int maxSecondaryNotifications = 10;

        private GameManager gameManager;

        private Control primaryParent;
        private Control secondaryParent;

        private UINotification primaryNotification;
        private List<UINotification> notifications = new List<UINotification>();

        public override void _Ready()
        {
            primaryParent = GetNode<VBoxContainer>("VBoxContainer");
            secondaryParent = primaryParent;

            this.TryGetGameManager(out gameManager);
        }

        public override void _EnterTree()
        {
            GameEvents.GameNotification.Connect(PushNotification);

            GameEvents.ShipSystemStateChanged.Connect(OnShipSystemStateChanged);
            GameEvents.ShipEnergyOverloading.Connect(OnShipEnergyOverloading);
            GameEvents.ShipEnergyOverloaded.Connect(OnShipEnergyOverloaded);
        }

        

        public override void _ExitTree()
        {
            GameEvents.GameNotification.Disconnect(PushNotification);

            GameEvents.ShipSystemStateChanged.Disconnect(OnShipSystemStateChanged);
            GameEvents.ShipEnergyOverloading.Disconnect(OnShipEnergyOverloading);
            GameEvents.ShipEnergyOverloaded.Disconnect(OnShipEnergyOverloaded);
        }

        private void PushNotification(Notification notification)
        {
            var newNotification = SpawnNotification(notification);
            if (newNotification is null)
            {
                GD.PrintErr("Failed to create notification instance.");
                return;
            }

            notifications.Insert(0, newNotification);

            RemoveExcessNotifications();
            RemoveChildrenFromParents();
            UpdateNotification();
        }

        private void OnNotificationReadyForRemoval()
        {
            RemoveOldNotifications();
            RemoveChildrenFromParents();
            UpdateNotification();
        }

        private UINotification SpawnNotification(Notification notification)
        {
            var instance = notificationScene.Instantiate<UINotification>();
            if (instance is not null)
            {
                instance.ReadyForRemoval += OnNotificationReadyForRemoval;
                instance.Setup(notification);
            }
            return instance;
        }

        private void RemoveOldNotifications()
        {
            for (int i = notifications.Count - 1; i > 0; i--)
            {
                if (notifications[i].IsReadyForRemoval)
                {
                    DestroyNotification(i);
                }
            }
        }

        private void RemoveExcessNotifications()
        {
            // Remove any notifications pass the limit max + 1 due to 
            // notifications also including the primary notification.
            while (notifications.Count > maxSecondaryNotifications)
            {
                DestroyNotification(notifications.Count - 1);
            }
        }

        private void DestroyNotification(int index)
        {
            var target = notifications[index];
            notifications.RemoveAt(index);

            target.ReadyForRemoval -= OnNotificationReadyForRemoval;
            target.QueueFree();
        }

        private void RemoveChildrenFromParents()
        {
            foreach (var notification in notifications)
            {
                if (notification.GetParent() is not null)
                {
                    notification.GetParent().RemoveChild(notification);
                }
            }
        }

        private void UpdateNotification()
        {
            if (notifications.Count == 0)
            {
                return;
            }

            primaryParent.AddChild(notifications[0]);

            for (int i = 1; i < notifications.Count; i++)
            {
                secondaryParent.AddChild(notifications[i]);
            }
        }

        private void OnShipSystemStateChanged(ShipSystemType type)
        {
            var system = gameManager.PlayerShip.GetSystem(type);
            var text = system.State switch {
                ShipSystemState.Damaged => $"{type} system damaged!",
                ShipSystemState.Destroyed => $"{type} system destroyed",
                _ => string.Empty
            };

            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            PushNotification(new Notification(text));
        }

        private void OnShipEnergyOverloading()
        {
            PushNotification(new Notification("Ship systems overloading!"));
        }

        private void OnShipEnergyOverloaded()
        {
            PushNotification(new Notification("Ship systems overloaded!"));
        }
    }
}