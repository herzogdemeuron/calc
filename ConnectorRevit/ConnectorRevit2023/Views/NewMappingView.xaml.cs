using Calc.ConnectorRevit.ViewModels;
using Calc.Core;
using Calc.Core.Objects;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

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

        private void NewNameTextChanged(object sender, TextChangedEventArgs e)
        {
            CheckCreateEnable();
        }

        private void MappingSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CheckCreateEnable();
        }

        private void CheckCreateEnable()
        {
            string newName = this.NewNameText.Text.Trim();
            if (newName == "")
            {
                this.CreateButton.IsEnabled = false;
                this.CreateButton.Content = "Please enter a name";
            }
            else if (currentNames.Contains(newName))
            {
                this.CreateButton.IsEnabled = false;
                this.CreateButton.Content = "Name already exists";
            }
            else if (this.MappingListBox.SelectedItem == null)
            {
                this.CreateButton.IsEnabled = false;
                this.CreateButton.Content = "Please select a mapping";
            }
            else
            {
                this.CreateButton.IsEnabled = true;
                this.CreateButton.Content = "Create";
            }

        }

        private async void CreateButtonClicked(object sender, RoutedEventArgs e)
        {
            Mapping selectedMapping = this.MappingListBox.SelectedItem as Mapping;
            string newName = this.NewNameText.Text.Trim();
            await viewModel.HandelNewMappingCreate(selectedMapping, newName);
            this.Close();
        }

    }
}
