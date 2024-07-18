using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Calc.Core.Interfaces;
using Calc.MVVM.Helpers.Mediators;
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
                MediatorFromVM.Reset();
                MediatorToView.Reset();

                App.RevitVersion = commandData.Application.Application.VersionNumber;
                UIDocument uidoc = commandData.Application.ActiveUIDocument;
                Document doc = commandData.Application.ActiveUIDocument.Document;

                LoginViewModel loginVM = new LoginViewModel(false,"Calc Builder Login");
                LoginView loginView = new LoginView(loginVM);
                loginView.ShowDialog();

                if (!loginVM.FullyPrepared) return Result.Cancelled;

                ElementSourceHandler elementSourceHandler = new ElementSourceHandler(uidoc);
                RevitImageCreator imageCreator = new RevitImageCreator(doc);
                ElementSender elementSender = new ElementSender(doc, loginVM.CalcStore.Config);
                BuilderViewModel builderViewModel = new BuilderViewModel(loginVM.CalcStore, elementSourceHandler, imageCreator, elementSender);
                BuilderView builderView = new BuilderView(builderViewModel);

                builderView.Show();

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                Logger.Log(ex.Message);
                Debug.WriteLine(ex.Message);
                return Result.Failed;
            }
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            // for general case
            string assemblyLocation = Assembly.GetExecutingAssembly().Location;
            // for pyrevit invoked case
            string assemblyLocationHdM = "C:\\HdM-DT\\RevitCSharpExtensions\\calc-test-bin\\bin";

            string assemblyFolder = string.IsNullOrEmpty(assemblyLocation) ?
                assemblyLocationHdM : Path.GetDirectoryName(assemblyLocation);
            string assemblyName = new AssemblyName(args.Name).Name;
            string assemblyPath = Path.Combine(assemblyFolder, assemblyName + ".dll");

            if (File.Exists(assemblyPath))
            {
                return Assembly.LoadFrom(assemblyPath);

            }
            else
            {
                return null;
            }
        }
    }
}