namespace SpaceEngineer
{
    /// <summary>
    /// A simple game event with no arguments.
    /// </summary>
    public class GameEvent
    {
        private event System.Action _event;

        public GameEvent() => _event = null;

        /// <summary>
        /// Emit the event to all registered callbacks.
        /// </summary>

        public void Emit() => _event?.Invoke();

        /// <summary>
        /// Remove all registered callbacks to the event.
        /// </summary>
        public void Clear() => _event = null;

        /// <summary>
        /// Register a callback to the event.
        /// </summary>
        public void Connect(System.Action callback) => _event += callback;

        /// <summary>
        /// Unregister a callback to the event.
        /// </summary>
        public void Disconnect(System.Action callback) => _event -= callback;
    }

    /// <summary>
    /// A simple game event with a single typed arguments.
    /// </summary>
    public class GameEvent<Arg1>
    {
        private event System.Action<Arg1> _event;

        public GameEvent() => _event = null;

        /// <summary>
        /// Emit the event to all registered callbacks.
        /// </summary>
        public void Emit(Arg1 data) => _event?.Invoke(data);

        /// <summary>
        /// Remove all registered callbacks to the event.
        /// </summary>
        public void Clear() => _event = null;

        /// <summary>
        /// Register a callback to the event.
        /// </summary>
        public void Connect(System.Action<Arg1> callback) => _event += callback;

        /// <summary>
        /// Unregister a callback to the event.
        /// </summary>
        public void Disconnect(System.Action<Arg1> callback) => _event -= callback;
    }

    /// <summary>
    /// A simple game event with two typed arguments.
    /// </summary>
    public class GameEvent<Arg1, Arg2>
    {
        private event System.Action<Arg1, Arg2> _event;

        public GameEvent() => _event = null;

        /// <summary>
        /// Emit the event to all registered callbacks.
        /// </summary>
        public void Emit(Arg1 arg1, Arg2 arg2) => _event?.Invoke(arg1, arg2);

        /// <summary>
        /// Remove all registered callbacks to the event.
        /// </summary>
        public void Clear() => _event = null;

        /// <summary>
        /// Register a callback to the event.
        /// </summary>
        public void Connect(System.Action<Arg1, Arg2> callback) => _event += callback;

        /// <summary>
        /// Unregister a callback to the event.
        /// </summary>
        public void Disconnect(System.Action<Arg1, Arg2> callback) => _event -= callback;
    }

    /// <summary>
    /// A simple game event with a three typed arguments.
    /// </summary>
    public class GameEvent<Arg1, Arg2, Arg3>
    {
        private event System.Action<Arg1, Arg2, Arg3> _event;

        public GameEvent() => _event = null;

        /// <summary>
        /// Emit the event to all registered callbacks.
        /// </summary>
        public void Emit(Arg1 arg1, Arg2 arg2, Arg3 arg3) => _event?.Invoke(arg1, arg2, arg3);

        /// <summary>
        /// Remove all registered callbacks to the event.
        /// </summary>
        public void Clear() => _event = null;

        /// <summary>
        /// Register a callback to the event.
        /// </summary>
        public void Connect(System.Action<Arg1, Arg2, Arg3> callback) => _event += callback;

        /// <summary>
        /// Unregister a callback to the event.
        /// </summary>
        public void Disconnect(System.Action<Arg1, Arg2, Arg3> callback) => _event -= callback;
    }
}