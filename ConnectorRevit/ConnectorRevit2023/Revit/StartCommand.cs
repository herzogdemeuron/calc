using System;
using System.IO;
using System.Diagnostics;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using Calc.ConnectorRevit.Views;
using Calc.Core;
using System.Reflection;
using Calc.ConnectorRevit.ViewModels;
using Calc.ConnectorRevit.Services;
using Calc.Core.DirectusAPI;
using System.Threading.Tasks;
using Calc.ConnectorRevit.Helpers;

namespace Calc.ConnectorRevit.Revit
{
    [Transaction(TransactionMode.Manual)]
    public class StartCommand : IExternalCommand
    {
        private Directus directusInstance;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                App.RevitVersion = commandData.Application.Application.VersionNumber;
                AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
                App.CurrentDoc = commandData.Application.ActiveUIDocument.Document;
                App.EventHandler = new ExternalEventHandler();
                Logger.Log("Now authenticating.");
                Task.Run(() => Authenticate()).Wait();
                if (directusInstance == null)
                {
                    Logger.Log("Failed to get directus.");
                    return Result.Cancelled;
                }
                Logger.Log("Authentication successful.");
                DirectusStore store = new DirectusStore(directusInstance);
                MainView mainView = new MainView(new MainViewModel(store));
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
            var authenticator = new DirectusAuthenticator();
            directusInstance = await authenticator.ShowLoginWindowAsync().ConfigureAwait(false);
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            // depending on the build mode, use different paths
            string assemblyFolder = @"C:\HdM-DT\calc\ConnectorRevit\ConnectorRevit2023\bin\Debug";
            //string assemblyFolder = $"C:\\ProgramData\\Autodesk\\Revit\\Addins\\{App.RevitVersion}\\CalcRevit"; // Specify the directory where your DLLs are located
            string assemblyName = new AssemblyName(args.Name).Name;
            string assemblyPath = Path.Combine(assemblyFolder, assemblyName + ".dll");

            if (File.Exists(assemblyPath))
                return Assembly.LoadFrom(assemblyPath);

            return null;
        }
    }
}