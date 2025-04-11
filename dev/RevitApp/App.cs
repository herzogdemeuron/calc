using Autodesk.Revit.UI;
using Calc.RevitApp.Revit;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Calc.RevitApp
{
    public class App : IExternalApplication
    {
        public static string RevitVersion { get; set; }
        public Result OnStartup(UIControlledApplication application)
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

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

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (args.Name.Contains(".resources"))
            {
                Debug.WriteLine("Ignoring resource satellite assembly resolve for: " + args.Name);
                return null;
            }

            string baseDirectory = "C:\\source\\calc\\bin\\net8.0-windows";

            string assemblyName = new AssemblyName(args.Name).Name;
            string assemblyPath = Path.Combine(baseDirectory, assemblyName + ".dll");

            Debug.WriteLine($"Attempting to resolve {assemblyName} at {assemblyPath}...");

            if (File.Exists(assemblyPath))
            {
                return Assembly.LoadFrom(assemblyPath);
            }
            else
            {
                Debug.WriteLine($"Assembly {assemblyName} not found at {assemblyPath}!");
                return null;
            }
        }

    }
}