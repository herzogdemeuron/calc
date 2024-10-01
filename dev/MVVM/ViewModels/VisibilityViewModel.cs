using System.ComponentModel;
using System.Windows;

namespace Calc.MVVM.ViewModels
{
    public class VisibilityViewModel : INotifyPropertyChanged
    {


        private string messageText;
        public string MessageText
        {
            get { return messageText; }
            set
            {
                messageText = value;
                OnPropertyChanged(nameof(MessageText));
            }
        }

        private string waitingText;
        public string WaitingText
        {
            get { return waitingText; }
            set
            {
                waitingText = value;
                OnPropertyChanged(nameof(WaitingText));
            }
        }

        private string savingmessage;
        public string SavingMessage
        {
            get { return savingmessage; }
            set
            {
                savingmessage = value;
                OnPropertyChanged(nameof(SavingMessage));
            }
        }
        private Visibility projectOverlayVisibility = Visibility.Visible;
        public Visibility ProjectOverlayVisibility
        {
            get { return projectOverlayVisibility; }
            set
            {
                projectOverlayVisibility = value;
                OnPropertyChanged(nameof(ProjectOverlayVisibility));
            }
        } 

        private Visibility waitingOverlayVisibility;
        public Visibility WaitingOverlayVisibility
        {
            get { return waitingOverlayVisibility; }
            set
            {
                waitingOverlayVisibility = value;
                OnPropertyChanged(nameof(WaitingOverlayVisibility));
            }
        }

        private Visibility messageOverlayVisibility;
        public Visibility MessageOverlayVisibility
        {
            get { return messageOverlayVisibility; }
            set
            {
                messageOverlayVisibility = value;
                OnPropertyChanged(nameof(MessageOverlayVisibility));
            }
        }

        private Visibility savingOverlayVisibility;
        public Visibility SavingOverlayVisibility
        {
            get { return savingOverlayVisibility; }
            set
            {
                savingOverlayVisibility = value;
                OnPropertyChanged(nameof(SavingOverlayVisibility));
            }
        }

        private Visibility newMappingOverlayVisibility;
        public Visibility NewMappingOverlayVisibility
        {
            get { return newMappingOverlayVisibility; }
            set
            {
                newMappingOverlayVisibility = value;
                OnPropertyChanged(nameof(NewMappingOverlayVisibility));
            }
        }

        private Visibility loginOverlayVisibility;
        public Visibility LoginOverlayVisibility
        {
            get { return loginOverlayVisibility; }
            set
            {
                loginOverlayVisibility = value;
                OnPropertyChanged(nameof(LoginOverlayVisibility));
            }
        }

        public VisibilityViewModel()
        {
            HideAllOverlays(false);
        }


        internal void ShowSavingOverlay(string message)
        {
            HideAllOverlays();
            SavingMessage = message;
            SavingOverlayVisibility = Visibility.Visible;

        }

        internal void ShowNewMappingOverlay()
        {
            HideAllOverlays();
            NewMappingOverlayVisibility = Visibility.Visible;
        }

        internal void ShowWaitingOverlay(string text)
        {
            HideAllOverlays();
            WaitingText = text;
            WaitingOverlayVisibility = Visibility.Visible;
        }

        internal void ShowMessageOverlay(bool? route, params object[] args)
        {
            
            MessageOverlayVisibility = Visibility.Visible;
            switch (route)
            {
                case null:
                    MessageText = args[0]?.ToString() ?? "Unknown";
                    break;
                case true:
                    MessageText = args[1]?.ToString() ?? "Unknown";
                    break;
                case false:
                    MessageText = args[2]?.ToString() ?? "Unknown";
                    break;
            }
        }

        internal void HideMessageOverlay() 
        {
            MessageOverlayVisibility = Visibility.Collapsed;
        }

        internal void HideAllOverlays(bool? hideProjects = true)
        {
            WaitingOverlayVisibility = Visibility.Collapsed;
            MessageOverlayVisibility = Visibility.Collapsed;
            SavingOverlayVisibility = Visibility.Collapsed;
            NewMappingOverlayVisibility= Visibility.Collapsed;
            if (hideProjects == true)
            {
                ProjectOverlayVisibility = Visibility.Collapsed;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
