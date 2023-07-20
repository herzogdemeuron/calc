using Calc.ConnectorRevit.Helpers;
using Calc.Core.Objects;
using Calc.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Autodesk.Revit.DB.Visual;

namespace Calc.ConnectorRevit.ViewModels
{

    public class VisibilityViewModel : INotifyPropertyChanged
    {

        private readonly DirectusStore store;

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
        private Visibility projectOverlayVisibility;
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

        public VisibilityViewModel(DirectusStore directusStore)
        {
            HideAllOverlays();
            store = directusStore;
            ViewMediator.Register("ShowMainView", _ => HideAllOverlays());
            ViewMediator.Register("ShowProjectOverlay", _ => ShowProjectOverlay());
            ViewMediator.Register("ShowWaitingOverlay", message => ShowWaitingOverlay((string)message));
            ViewMediator.Register("ShowSavingOverlay", message => ShowSavingOverlay((string)message));
            ViewMediator.Register("ShowMessageOverlay", param => {
                var tuple = (ValueTuple<bool?, List<string>>)param;
                ShowMessageOverlay(tuple.Item1, tuple.Item2);
            });

        }

        private void ShowProjectOverlay()
        {
            HideAllOverlays();
            ProjectOverlayVisibility = Visibility.Visible;
        }

        private void ShowSavingOverlay(string message)
        {
            HideAllOverlays();
            SavingMessage = message;
            SavingOverlayVisibility = Visibility.Visible;

        }

        private void ShowWaitingOverlay(string text)
        {
            HideAllOverlays();
            WaitingText = text;
            WaitingOverlayVisibility = Visibility.Visible;
        }

        private void ShowMessageOverlay(bool? result, List<string> messages)
        {
            HideAllOverlays();
            MessageOverlayVisibility = Visibility.Visible;
            if (result == null)
            {
                MessageText = messages.First();
            }
            else if (result == true)
            {
                MessageText = messages[1];
            }
            else
            {
                MessageText = messages.Last();
            }
        }

        private void HideAllOverlays()
        {
            WaitingOverlayVisibility = Visibility.Collapsed;
            MessageOverlayVisibility = Visibility.Collapsed;
            SavingOverlayVisibility = Visibility.Collapsed;
            ProjectOverlayVisibility = Visibility.Collapsed;
        }
        


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
