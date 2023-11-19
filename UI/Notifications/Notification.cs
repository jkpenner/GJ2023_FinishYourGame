namespace SpaceEngineer
{
    public class Notification
    {
        public readonly string Message;
        public readonly float DisplayTime;

        public Notification(string message, float displayTime = 2f)
        {
            Message = message;
            DisplayTime = displayTime;
        }
    }
}