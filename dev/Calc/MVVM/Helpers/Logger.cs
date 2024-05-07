using System;

namespace Calc.MVVM.Helpers
{
    public  class Logger
    {
        static string filePath = @"C:\log.txt";
        public static void Log(string message)
        {
            string time = DateTime.Now.ToString("[MM-dd HH:mm:ss] ");
            System.IO.File.AppendAllText(filePath, '\n' + time + message);
        }
    }
}
