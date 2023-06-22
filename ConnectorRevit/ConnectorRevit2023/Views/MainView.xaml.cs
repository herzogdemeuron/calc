using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Autodesk.Revit.DB;
using Calc.ConnectorRevit.Views;


namespace Calc.ConnectorRevit.Views
{
    public partial class MainView : Window
    {
        private readonly ViewModel _viewModel;
        public MainView()
        {
            _viewModel = App.ViewModel;
            DataContext = _viewModel;
            InitializeComponent();
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            _viewModel.SelectedItem = e.NewValue as TreeViewItem;
            _viewModel.SetView();
        }

        private void Calculate_Click(object sender, RoutedEventArgs e)
        {
            //_viewModel.Calculate();
        }

        private void ResetVisualization_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.ResetView();
        }
    }
}
