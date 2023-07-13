using Calc.Core;
using Calc.Core.Objects;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Calc.ConnectorRevit.Views
{
    public partial class NewMappingView : Window
    {
        private DirectusStore store;
        private NewMappingViewModel viewModel;
        private List<string> currentNames
        {
            get => store.MappingsProjectRelated.Select(m => m.Name).ToList();
        }
        public NewMappingView(DirectusStore directusStore)
        {
            store = directusStore;
            InitializeComponent();
            viewModel = new NewMappingViewModel(store);
            DataContext = viewModel;
        }

        private void NewNameTextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            string newName = this.NewNameText.Text.Trim();
            if(newName == "")
            {
                this.CreateButton.IsEnabled = false;
                this.CreateButton.Content = "Please enter a name";
            }
            else if (currentNames.Contains(newName))
            {
                this.CreateButton.IsEnabled = false;
                this.CreateButton.Content = "Name already exists";
            }
            else
            {
                this.CreateButton.IsEnabled = true;
                this.CreateButton.Content = "Create";
            }
        }

        private void CreateButtonClicked(object sender, RoutedEventArgs e)
        {
            viewModel.HandelNewMappingCreate();
        }

    }
}
