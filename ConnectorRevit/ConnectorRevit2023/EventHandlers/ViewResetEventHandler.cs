using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using Calc.ConnectorRevit;
using System;
using System.Diagnostics;
using Calc.Core.Objects;
using Calc.ConnectorRevit.Views;
using Calc.ConnectorRevit.Revit;

namespace Calc.ConnectorRevit.EventHandlers
{ 
    [Transaction(TransactionMode.Manual)]
    public class ViewResetEventHandler : IExternalEventHandler
    {
        private readonly ViewModel _viewModel = App.ViewModel;
        public Branch SelectedBranch
        {
            get
            {
                return _viewModel.SelectedBranch;
            }
        }

        public ViewResetEventHandler(ViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public void Execute(UIApplication uiapp)
        {
            try
            {
                Visualizer.ShowAll();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        public string GetName()
        {
            return "ResetEventHandler";
        }
    }
}