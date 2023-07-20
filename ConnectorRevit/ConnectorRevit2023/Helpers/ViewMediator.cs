using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calc.ConnectorRevit.Helpers
{
    public class ViewMediator
    {
        /// <summary>
        /// Broadcasts messages to main view
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
                foreach (var callback in dictionary[token])
                {
                    callback(args);
                }
            }
        }
    }
}
