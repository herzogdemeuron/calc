using Autodesk.Revit.UI;
using Calc.RevitApp.Revit;
using System;
using System.IO;
using System.Reflection;

namespace Calc.RevitApp
{
    public class App : IExternalApplication
    {
        public static string RevitVersion { get; set; }
        public Result OnStartup(UIControlledApplication application)
        {
            AppDomain.CurrentDomain.AssemblyResolve += OnResolveAssembly;

            RibbonMaker.Create(application, "CALC");
            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        Assembly OnResolveAssembly(object sender, ResolveEventArgs args)
        {
            string directoryDLLs = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string pathAssembly = Path.Combine(directoryDLLs, new AssemblyName(args.Name).Name + ".dll");
            if (File.Exists(pathAssembly))
            {
                return Assembly.LoadFrom(pathAssembly);
            }
            return null;
        }

    }
}