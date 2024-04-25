using Calc.MVVM.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Calc.MVVM.Views
{
    public partial class MaterialSelectionView : Window
    {
        private readonly LoginViewModel LoginVM;

        public MaterialSelectionView(LoginViewModel viewModel)
        {
            this.LoginVM = viewModel;
            this.DataContext = LoginVM;
            InitializeComponent();
        }
            

        private async void LoginOkClicked(object sender, RoutedEventArgs e)
        {
            bool directusAuth = await LoginVM.AuthenticateAndLoad();
            if (directusAuth) this.Close();
        }

        private void LoginQuitClicked(object sender, RoutedEventArgs e)
        {
            LoginVM.CancelLoad();
            this.Close();
        }



    }
}
