using System;
using System.Diagnostics;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using Calc.Core.Objects;
using Calc.ConnectorRevit.EventHandlers;
using Calc.ConnectorRevit.Views;

namespace Calc.ConnectorRevit.EventHandlers
{
    [Transaction(TransactionMode.Manual)]
    public class ViewSetEventHandler : IExternalEventHandler
    {
        private readonly ViewModel _viewModel;
        private readonly ExternalEvent _exEvent;
        public Branch SelectedBranch
        {
            get
            {
                return _viewModel.SelectedBranch;
            }
        }

        public ViewSetEventHandler(ViewModel viewModel)
        {
            _viewModel = viewModel;
            _exEvent = ExternalEvent.Create(this);
        }

        public void Execute(UIApplication uiapp)
        {
            try
            {
                Visualizer.IsolateAndColor();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        public void Raise()
        {
            _exEvent.Raise();
        }

        public string GetName()
        {
            return "VisualizeEventHandler";
        }
    }
}


