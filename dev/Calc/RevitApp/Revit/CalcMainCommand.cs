using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Calc.Core;
using Calc.Core.DirectusAPI;
using Calc.MVVM.ViewModels;
using Calc.MVVM.Views;
using Calc.RevitConnector.Revit;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Calc.RevitApp.Revit
{
    [Transaction(TransactionMode.Manual)]
    public class CalcMainCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                App.RevitVersion = commandData.Application.Application.VersionNumber;
                AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
                Document doc = commandData.Application.ActiveUIDocument.Document;

                LoginViewModel loginVM = new LoginViewModel(true, "Calc Login");
                LoginView loginView = new LoginView(loginVM);
                loginView.ShowDialog();

                RevitElementCreator elementCreator = new RevitElementCreator(doc);
                RevitVisualizer visualizer = new RevitVisualizer( doc, new RevitExternalEventHandler());
                MainViewModel mainViewModel = new MainViewModel(loginVM.DirectusStore, elementCreator, visualizer);
                MainView mainView = new MainView(mainViewModel);

                mainView.Show();
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                Logger.Log(ex.Message);
                Debug.WriteLine(ex.Message);
                return Result.Failed;
            }
        }

        private async Task Authenticate()
        {
            //var authenticator = new DirectusAuthenticator();
            //directusInstance = await authenticator.ShowLoginWindowAsync().ConfigureAwait(false);
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            // depending on the build mode, use different paths
            string assemblyFolder = @"C:\source\calc\ConnectorRevit\ConnectorRevit2023\bin\Debug";
            //string assemblyFolder = $"C:\\ProgramData\\Autodesk\\Revit\\Addins\\{App.RevitVersion}\\CalcRevit"; // Specify the directory where your DLLs are located
            string assemblyName = new AssemblyName(args.Name).Name;
            string assemblyPath = Path.Combine(assemblyFolder, assemblyName + ".dll");

            if (File.Exists(assemblyPath))
                return Assembly.LoadFrom(assemblyPath);

            return null;
        }
    }
}