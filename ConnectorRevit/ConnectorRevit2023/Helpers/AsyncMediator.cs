using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calc.ConnectorRevit.Helpers
{
    public class AsyncMediator
    {
        private static IDictionary<string, List<Func<object, Task>>> dictionary = new Dictionary<string, List<Func<object, Task>>>();

        public static void Register(string token, Func<object, Task> callback)
        {
            if (!dictionary.ContainsKey(token))
            {
                dictionary[token] = new List<Func<object, Task>>();
            }
            dictionary[token].Add(callback);
        }

        public static void Unregister(string token, Func<object, Task> callback)
        {
            if (dictionary.ContainsKey(token))
            {
                dictionary[token].Remove(callback);
            }
        }

        public static async Task Broadcast(string token, object args = null)
        {
            if (dictionary.ContainsKey(token))
            {
                foreach (var callback in dictionary[token])
                {
                    await callback(args);
                }
            }
        }
    }
}
