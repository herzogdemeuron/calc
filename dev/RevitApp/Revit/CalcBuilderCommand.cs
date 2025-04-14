using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Calc.MVVM.ViewModels;
using Calc.MVVM.Views;
using Calc.RevitConnector.Revit;
using SpeckleSender;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Calc.RevitApp.Revit
{
    [Transaction(TransactionMode.Manual)]
    public class CalcBuilderCommand : IExternalCommand
    {
        static CalcBuilderCommand()
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
                UIDocument uidoc = commandData.Application.ActiveUIDocument;
                Document doc = commandData.Application.ActiveUIDocument.Document;

                LoginViewModel loginVM = new LoginViewModel(false,"Calc Builder Login");
                LoginView loginView = new LoginView(loginVM);
                loginView.ShowDialog();

                if (!loginVM.FullyPrepared) return Result.Cancelled;

                ElementSourceHandler elementSourceHandler = new ElementSourceHandler(uidoc, new RevitExternalEventHandler());
                RevitImageCreator imageCreator = new RevitImageCreator(doc);
                ElementSender elementSender = new ElementSender(doc, loginVM.CalcStore.Config, App.RevitVersion);
                BuilderViewModel builderViewModel = new BuilderViewModel(loginVM.CalcStore, elementSourceHandler, imageCreator, elementSender);
                CalcBuilderView builderView = new CalcBuilderView(builderViewModel);

                builderView.Show();

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Error", ex.Message);
                Debug.WriteLine(ex.Message);
                return Result.Failed;
            }
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            string baseDirectory = "C:\\HdM-DT\\RevitCSharpExtensions\\calc-test-bin\\bin\\net48";
            // if revit version is greater than 2024, do not resolve
            if (App.RevitVersion > 2024)
            {
                baseDirectory = "C:\\HdM-DT\\RevitCSharpExtensions\\calc-test-bin\\bin\\net8.0-windows";
            }
            if (args.Name.Contains(".resources"))
            {
                Debug.WriteLine("Ignoring resource satellite assembly resolve for: " + args.Name);
                return null;
            }
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