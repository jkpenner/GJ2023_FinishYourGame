using System;
using System.Collections.Generic;
using Godot;

namespace SpaceEngineer
{
    public partial class AirLock : Node3D
    {
        [Export] Godot.Collections.Array<Station> outputStations;
        [ExportGroup("Debug")]
        [Export] bool debugInfinteOutput = false;
        [Export] Godot.Collections.Array<Item> debugOutputItems;

        private Random random = new Random();
        private Queue<Item> queuedItems = new Queue<Item>();

        public override void _Process(double delta)
        {
            foreach(var output in outputStations)
            {
                if (output.HeldItem is not null)
                {
                    continue;
                }

                if (queuedItems.Count > 0)
                {
                    var item = queuedItems.Dequeue();
                    output.TryPlaceObject(item);
                }
                else if (debugInfinteOutput && debugOutputItems.Count > 0)
                {
                    var item = debugOutputItems[random.Next(0, debugOutputItems.Count)];
                    output.TryPlaceObject(item);
                }
            }
        }
    }
}