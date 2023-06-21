using System;
using System.Diagnostics;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using Calc.Core.Objects;

namespace Calc.ConnectorRevit
{
    [Transaction(TransactionMode.Manual)]
    public class VisualizeEventHandler : IExternalEventHandler
    {
        private readonly MainViewModel _viewModel;
        public Branch SelectedBranch
        {
            get
            {
                return _viewModel.SelectedBranch;
            }
        }

        public VisualizeEventHandler(MainViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public void Execute(UIApplication uiapp)
        {
            Visualizer visualizer = new Visualizer(uiapp, SelectedBranch);
            try
            {
                visualizer.IsolateAndColor();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        public string GetName()
        {
            return "VisualizeEventHandler";
        }
    }

    [Transaction(TransactionMode.Manual)]
    public class ResetVisualizetionEventHandler : IExternalEventHandler
    {
        private readonly MainViewModel _viewModel;
        public Branch SelectedBranch
        {
            get
            {
                return _viewModel.SelectedBranch;
            }
        }

        public ResetVisualizetionEventHandler(MainViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public void Execute(UIApplication uiapp)
        {
            Visualizer visualizer = new Visualizer(uiapp, _viewModel);
            try
            {
                visualizer.ShowAll();
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

