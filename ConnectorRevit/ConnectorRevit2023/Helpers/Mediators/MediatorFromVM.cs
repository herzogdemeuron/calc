using System;
using System.Collections.Generic;

namespace Calc.ConnectorRevit.Helpers.Mediators
{
    public class MediatorFromVM
    {
        /// <summary>
        /// Broadcasts messages from source to ViewModels
        /// The events are named from the source view model 
        /// Broadcast happens in the source view model, Register and Unregister happens in the target view model
        /// ViewModels => NodeTreeViewModel(triggered) => RevitVisualizer(triggered) 
        /// </summary>
        private static IDictionary<string, List<Action<object>>> dictionary = new Dictionary<string, List<Action<object>>>();

        public static void Register(string token, Action<object> callback)
        {
            if (!dictionary.ContainsKey(token))
            {
                dictionary[token] = new List<Action<object>>();
            }
            dictionary[token].Add(callback);
        }

        public static void Unregister(string token, Action<object> callback)
        {
            if (dictionary.ContainsKey(token))
            {
                dictionary[token].Remove(callback);
            }
        }
        public static void Broadcast(string token, object args = null)
        {
            if (dictionary.ContainsKey(token))
            {
                var callbacks = new List<Action<object>>(dictionary[token]);
                foreach (var callback in callbacks)
                {
                    callback(args);
                }
            }
        }
    }
}
