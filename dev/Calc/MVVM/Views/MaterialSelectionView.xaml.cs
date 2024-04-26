using Calc.Core.Objects.Materials;
using Calc.MVVM.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Calc.MVVM.Views
{
    public partial class MaterialSelectionView : Window
    {
        private readonly MaterialSelectionViewModel MaterialSelectionVM;
        public Material SelectedMaterial { get => MaterialSelectionVM.SelectedMaterial; }

        public MaterialSelectionView(MaterialSelectionViewModel materialSelectionVM)
        {
            this.MaterialSelectionVM = materialSelectionVM;
            this.DataContext = MaterialSelectionVM;
            InitializeComponent();
        }

        private void OKClicked(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void CancelClicked(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
