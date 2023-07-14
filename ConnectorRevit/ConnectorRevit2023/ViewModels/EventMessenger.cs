using System;

namespace Calc.ConnectorRevit.ViewModels
{
    public static class EventMessenger
    {
        public static event Action<string> OnMessageReceived;

        public static void SendMessage(string message)
        {
            OnMessageReceived?.Invoke(message);
        }
    }
}
