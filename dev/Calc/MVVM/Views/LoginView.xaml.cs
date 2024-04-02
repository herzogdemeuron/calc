using Calc.Core.Objects;
using Calc.Core.Objects.Buildups;
using Calc.MVVM.Helpers;
using Calc.MVVM.Helpers.Mediators;
using Calc.MVVM.Models;
using Calc.MVVM.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Calc.MVVM.Views
{
    public partial class LoginView : Window
    {
        private readonly LoginViewModel LoginVM;

        public LoginView(LoginViewModel viewModel)
        {
            this.LoginVM = viewModel;
            this.DataContext = LoginVM;
            InitializeComponent();
            this.PasswordBox.Password = LoginVM.Password;
        }
            
        private async void WindowLoaded(object sender, RoutedEventArgs e)
        {
            //await BuilderVM.HandleWindowLoadedAsync();
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //BuilderVM.HandleWindowClosing();
        }

        private void PasswordChanged(object sender, RoutedEventArgs e)
        {
          LoginVM.Password = PasswordBox.Password;
        }

        private async void LoginOkClicked(object sender, RoutedEventArgs e)
        {
            bool directusAuth = await LoginVM.AuthenticateAndLoad();
            if (directusAuth) this.Close();
        }

        private void LoginQuitClicked(object sender, RoutedEventArgs e)
        {
            this.Close();
        }



    }
}
