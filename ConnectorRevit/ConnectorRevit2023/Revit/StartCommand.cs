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
                Task.Run(() => Authenticate()).Wait();
                if (directusInstance == null)
                {
                    Debug.WriteLine("Failed to get directus.");
                    return Result.Cancelled;
                }
                DirectusStore store = new DirectusStore(directusInstance);
                MainView mainView = new MainView(new ViewModel(store));
                App.View = mainView;
                mainView.Show();
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return Result.Failed;
            }
        }

        private async Task Authenticate()
        {
            var directus = null as Directus;
            try
            {
                var authenticator = new DirectusAuthenticator();
                directus = await authenticator.ShowLoginWindowAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            if (directus == null)
            {
                // end application
                System.Windows.Application.Current.Shutdown();
            }
            else
            {
                directusInstance = directus;
            }

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