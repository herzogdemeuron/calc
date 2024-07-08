using Calc.Core;
using Calc.Core.DirectusAPI;
using Calc.Core.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Calc.MVVM.ViewModels
{

    public class LoginViewModel : INotifyPropertyChanged
    {
        private readonly bool MainOrBuilder;
        public Directus DirectusInstance { get; set; }
        public CalcStore CalcStore { get; set; }
        public string Title { get; set; }
        public string Password { get; set; }
        private readonly CancellationTokenSource cTokenSource = new CancellationTokenSource();
        public bool FullyPrepared => DirectusInstance.Authenticated && CalcStore.AllDataLoaded;
        private bool authenticated = false;

        private bool canOK = true;
        public bool CanOK
        {
            get => canOK;
            set
            {
                canOK = value;
                OnPropertyChanged(nameof(CanOK));
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

        private Visibility selectionVisibility = Visibility.Collapsed;
        public Visibility SelectionVisibility
        {
            get => selectionVisibility;
            set
            {
                selectionVisibility = value;
                OnPropertyChanged(nameof(SelectionVisibility));
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

        private List<IShowName> selectionList;
        public List<IShowName> SelectionList
        {
            get => selectionList;
            set
            {
                selectionList = value;
                OnPropertyChanged(nameof(SelectionList));
            }
        }

        private IShowName selected;
        public IShowName Selected
        {
            get => selected;
            set
            {
                selected = value;
                OnPropertyChanged(nameof(Selected));
            }
        }

        private string selectionText;
        public string SelectionText
        {
            get => selectionText;
            set
            {
                selectionText = value;
                OnPropertyChanged(nameof(SelectionText));
            }
        }


        public LoginViewModel(bool mainOrBuilder, string title)
        {
            MainOrBuilder = mainOrBuilder;
            DirectusInstance = new Directus();
            Url = Properties.Settings.Default.Url;
            Email = Properties.Settings.Default.Email;
            Password = Properties.Settings.Default.Password;
            Title = title;
        }

        private async Task Authenticate()
        {
            await DirectusInstance.Authenticate(Url, Email, Password);
            if (!DirectusInstance.Authenticated)
            {
                throw new Exception("Authentication failed.");
            }

            DateTime timeNow = DateTime.Now;

            Properties.Settings.Default.Url = Url;
            Properties.Settings.Default.Email = Email;
            Properties.Settings.Default.Password = Password;
            Properties.Settings.Default.LastTime = timeNow;
            Properties.Settings.Default.Save();
        }

        private async Task LoadData()
        {
            CalcStore = new CalcStore(DirectusInstance);

            CalcStore.ProgressChanged += (s, p) =>
            {
                Dispatcher.CurrentDispatcher.Invoke(() =>
                {
                    ProgressValue = p;
                });
            };

            if (MainOrBuilder)
            {
                await CalcStore.GetMainData(cTokenSource.Token);
            }
            else
            {
                await CalcStore.GetBuilderData(cTokenSource.Token);
            }
        }

        public async Task<bool> HandleAutoLogin()
        {
            DateTime lastTime = Properties.Settings.Default.LastTime;
            // if the current time is within 1 hour of the last time, auto login (only for the same session)
            if (DateTime.Now.Subtract(lastTime).TotalHours < 1)
            {
                return await HandleOK();
            }
            
            else
            {
                return false;
            }
        }

        /// <summary>
        /// handles the ok button click in 2 stages: 1. auth and load data, 2. select project(for main login)
        /// returns true if the login is successful
        /// </summary>
        /// <returns></returns>
        public async Task<bool> HandleOK()
        {

            // auth firstly
            if (!authenticated)
            {
                CanOK = false;

                try
                {
                    await Authenticate();
                    authenticated = true;

                    LoadVisibility = Visibility.Visible;
                    LoginVisibility = Visibility.Collapsed;
                    await LoadData();
                    LoadVisibility = Visibility.Collapsed;
                }
                catch (Exception ex)
                {
                    Message = ex.Message;
                    CanOK = true;
                    LoginVisibility = Visibility.Visible;
                    return false;
                }

                // for builder directly return true
                if (!MainOrBuilder) return true;

                // for main prepare for project selection
                CanOK = true;
                SelectionList = CalcStore.ProjectsAll.OfType<IShowName>().ToList();
                SelectionText = "Select Project:";
                SelectionVisibility = Visibility.Visible;
                LoginVisibility = Visibility.Collapsed;
                return false;
            }

            // select project for main login
            if (MainOrBuilder)
            {
                if (Selected != null)
                {
                    var project = (Project)Selected;
                    CalcStore.ProjectSelected = project;
                    return true;
                }
                else
                {
                    Message = "Please select an item.";
                    return false;
                }
            }

            return false;

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