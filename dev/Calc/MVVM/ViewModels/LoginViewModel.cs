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
        public DirectusStore DirectusStore { get; set; }
        public string Title { get; set; }
        public string Password { get; set; }
        private readonly CancellationTokenSource cTokenSource = new CancellationTokenSource();
        public bool FullyPrepared => DirectusInstance.Authenticated && DirectusStore.AllDataLoaded;
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
            Url = Properties.Settings.Default.Config1;
            Email = Properties.Settings.Default.Config2;
            Password = Properties.Settings.Default.Config3;
            Title = title;
        }

        private async Task<bool> Authenticate()
        {
            await DirectusInstance.Authenticate(Url, Email, Password);
            if (!DirectusInstance.Authenticated) return false;

            Properties.Settings.Default.Config1 = Url;
            Properties.Settings.Default.Config2 = Email;
            Properties.Settings.Default.Config3 = Password;
            Properties.Settings.Default.Save();
            return true;
        }

        private async Task LoadData()
        {
            DirectusStore = new DirectusStore(DirectusInstance);

            DirectusStore.ProgressChanged += (s, p) =>
            {
                Dispatcher.CurrentDispatcher.Invoke(() =>
                {
                    ProgressValue = p;
                });
            };

            if (MainOrBuilder)
            {
                await DirectusStore.GetMainData(cTokenSource.Token);
            }
            else
            {
                await DirectusStore.GetBuilderData(cTokenSource.Token);
            }
        }

        private void PrepareSelection()
        {
            if(MainOrBuilder)
            {
                SelectionList = DirectusStore.ProjectsAll.OfType<IShowName>().ToList();
                SelectionText = "Select Project:";
            }
            else
            {
                SelectionList = DirectusStore.StandardsAll.OfType<IShowName>().ToList();
                SelectionText = "Select LCA Standard:";
            }
        }

        public async Task<bool> HandleOK()
        {

            if (!authenticated)
            {
                CanOK = false;
                try
                {
                    authenticated = await Authenticate();

                    if (authenticated)
                    {
                        LoginVisibility = Visibility.Collapsed;
                        LoadVisibility = Visibility.Visible;

                        await LoadData();

                        LoadVisibility = Visibility.Collapsed;
                        SelectionVisibility = Visibility.Visible;

                        PrepareSelection();
                    }
                }
                catch (Exception ex)
                {
                    Message = ex.Message;
                }

                CanOK = true;
                return false;
            }

            if (selected != null)
            {
                SetSelection();
                return true;
            }
            else
            {
                Message = "Please select an item.";
                return false;
            }

        }

        private void SetSelection()
        {
            if (MainOrBuilder)
            {
                var project = (Project)Selected;
                var standard = project.Standard;
                DirectusStore.ProjectSelected = project;
                DirectusStore.StandardSelected = standard;
            }
            else
            {
                DirectusStore.StandardSelected = (LcaStandard)Selected;
            }
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