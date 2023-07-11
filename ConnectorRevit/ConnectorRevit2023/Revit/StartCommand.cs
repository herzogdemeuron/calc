using System;
using System.IO;
using System.Diagnostics;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using Calc.ConnectorRevit.Views;
using Calc.Core;
using System.Reflection;

namespace Calc.ConnectorRevit.Revit
{
    [Transaction(TransactionMode.Manual)]
    public class StartCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
                App.CurrentDoc = commandData.Application.ActiveUIDocument.Document;
                App.ViewModel = new ViewModel();
                MainView mainView = new MainView();
                mainView.Show();
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return Result.Failed;
            }
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            string assemblyFolder = @"C:\ProgramData\Autodesk\Revit\Addins\2023\CalcRevit\Release"; // Specify the directory where your DLLs are located

            string assemblyName = new AssemblyName(args.Name).Name;
            string assemblyPath = Path.Combine(assemblyFolder, assemblyName + ".dll");

            if (File.Exists(assemblyPath))
                return Assembly.LoadFrom(assemblyPath);

            return null;
        }
    }
}