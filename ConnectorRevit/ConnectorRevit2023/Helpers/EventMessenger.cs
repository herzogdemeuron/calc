using System;

namespace Calc.ConnectorRevit.Helpers
{
    public static class EventMessenger
    {
        //sends messeges to the view from viewmodels
        public static event Action<string> OnMessageReceived;

        public static void SendMessage(string message)
        {
            OnMessageReceived?.Invoke(message);
        }
    }
}
