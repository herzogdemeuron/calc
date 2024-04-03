using Calc.Core;
using Calc.Core.DirectusAPI;
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Calc.MVVM.ViewModels
{

    public class LoginViewModel : INotifyPropertyChanged
    {
        public Directus DirectusInstance { get; set; }
        public DirectusStore DirectusStore { get; set; }
        public string Title { get; set; }
        public string Password { get; set; }
        private readonly CancellationTokenSource cTokenSource = new CancellationTokenSource();
        public bool FullyPrepared => DirectusInstance.Authenticated && DirectusStore.AllDataLoaded;

        private bool isNotSending = true;
        public bool IsNotSending
        {
            get => isNotSending;
            set
            {
                isNotSending = value;
                OnPropertyChanged(nameof(IsNotSending));
            }
        }

        private Visibility loginVisibility = Visibility.Visible;
        public Visibility LoginVisibility
        {
            get => loginVisibility;
            set
            {
                loginVisibility = value;
                OnPropertyChanged(nameof(LoginVisibility));
            }
        }

        private Visibility loadVisibility = Visibility.Collapsed;
        public Visibility LoadVisibility
        {
            get => loadVisibility;
            set
            {
                loadVisibility = value;
                OnPropertyChanged(nameof(LoadVisibility));
            }
        }

        private string message;
        public string Message
        {
            get => message;
            set
            {
                message = value;
                OnPropertyChanged(nameof(Message));
            }
        }

        private string url;
        public string Url
        {
            get => url;
            set
            {
                url = value.Trim();
                OnPropertyChanged(nameof(Url));
            }
        }

        private string email;
        public string Email
        {
            get => email;
            set
            {
                email = value.Trim();
                OnPropertyChanged(nameof(Email));
            }
        }

        private int progressValue;
        public int ProgressValue
        {
            get => progressValue;
            set
            {
                progressValue = value;
                OnPropertyChanged(nameof(ProgressValue));
            }
        }


        public LoginViewModel(string title)
        {
            DirectusInstance = new Directus();
            Url = Properties.Settings.Default.Config1;
            Email = Properties.Settings.Default.Config2;
            Password = Properties.Settings.Default.Config3;
            Title = title;
        }

        public async Task<bool> AuthenticateAndLoad()
        {
            try
            {
                IsNotSending = false;
                await DirectusInstance.Authenticate(Url, Email, Password);
                if (!DirectusInstance.Authenticated) return false;

                Properties.Settings.Default.Config1 = Url;
                Properties.Settings.Default.Config2 = Email;
                Properties.Settings.Default.Config3 = Password;
                Properties.Settings.Default.Save();

                LoginVisibility = Visibility.Collapsed;
                LoadVisibility = Visibility.Visible;

                await LoadData();

                return true;
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                return false;
            }
            finally
            {
                IsNotSending = true;
            }
        }

        public async Task LoadData()
        {
            DirectusStore = new DirectusStore(DirectusInstance);
            DirectusStore.ProgressChanged += (s, p) =>
            {
                Dispatcher.CurrentDispatcher.Invoke(() =>
                {
                    ProgressValue = p;
                });
            };
            await DirectusStore.GetBuilderData(cTokenSource.Token);
        }

        public void CancelLoad()
        {
            cTokenSource.Cancel();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}