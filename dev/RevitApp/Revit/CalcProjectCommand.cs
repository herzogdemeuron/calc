using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Calc.MVVM.ViewModels;
using Calc.MVVM.Views;
using Calc.RevitConnector.Revit;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Calc.RevitApp.Revit
{
    [Transaction(TransactionMode.Manual)]
    public class CalcProjectCommand : IExternalCommand
    {
        static CalcProjectCommand()
        {
            // dependencies are resolved at the point of command loading rather than command execution
            // so we implement the assembly resolve event here
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

        }

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                string version = commandData.Application.Application.VersionNumber;
                App.RevitVersion = int.Parse(version);

                Document doc = commandData.Application.ActiveUIDocument.Document;

                LoginViewModel loginVM = new LoginViewModel(true, "Calc Project Login");
                LoginView loginView = new LoginView(loginVM);
                loginView.ShowDialog();

                if (!loginVM.FullyPrepared) return Result.Cancelled;

                RevitElementCreator elementCreator = new RevitElementCreator(doc);
                RevitVisualizer visualizer = new RevitVisualizer( doc, new RevitExternalEventHandler());
                ProjectViewModel projectViewModel = new ProjectViewModel(loginVM.CalcStore, elementCreator, visualizer);
                CalcProjectView projectView = new CalcProjectView(projectViewModel);

                projectView.Show();
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                TaskDialog.Show("Error", ex.Message.ToString());
                return Result.Failed;
            }
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            // if revit version is greater than 2024, do not resolve
            if (App.RevitVersion > 2024) return null;
            if (args.Name.Contains(".resources"))
            {
                Debug.WriteLine("Ignoring resource satellite assembly resolve for: " + args.Name);
                return null;
            }
            string baseDirectory = "C:\\HdM-DT\\RevitCSharpExtensions\\calc-test-bin\\bin\\net48";
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