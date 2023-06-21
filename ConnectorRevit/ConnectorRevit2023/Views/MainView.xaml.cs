using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Autodesk.Revit.DB;


namespace Calc.ConnectorRevit
{
    public partial class MainView : Window
    {
        private readonly MainViewModel _mainViewModel;

        public MainView(Document doc)
        {
            _mainViewModel = new MainViewModel(doc);

            //_mainViewModel.GetAllBuildups();
            DataContext = _mainViewModel;
            InitializeComponent();
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            _mainViewModel.SelectedItem = e.NewValue as TreeViewItem;
            _mainViewModel.Visualize();
        }

        private void Calculate_Click(object sender, RoutedEventArgs e)
        {
            _mainViewModel.Calculate();
        }

        private void ResetVisualization_Click(object sender, RoutedEventArgs e)
        {
            _mainViewModel.ResetVisualization();
        }
    }
}
