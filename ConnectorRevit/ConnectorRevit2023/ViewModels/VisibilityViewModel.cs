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

        public VisibilityViewModel()
        {
            HideAllOverlays(false);
            ViewMediator.Register("ShowMainView", _ => HideAllOverlays());
            ViewMediator.Register("ShowProjectOverlay", _ => ShowProjectOverlay());
            ViewMediator.Register("ShowWaitingOverlay", message => ShowWaitingOverlay((string)message));
            ViewMediator.Register("ShowSavingOverlay", message => ShowSavingOverlay((string)message));
            ViewMediator.Register("ShowMessageOverlay", args => ShowMessageOverlay((List<object>)args));
            ViewMediator.Register("HideMessageOverlay", _ => HideMessageOverlay());

            ViewMediator.Register("ShowNewMappingOverlay", _ => ShowNewMappingOverlay());
            
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

        private void ShowNewMappingOverlay()
        {
            HideAllOverlays();
            NewMappingOverlayVisibility = Visibility.Visible;
        }

        private void ShowWaitingOverlay(string text)
        {
            HideAllOverlays();
            WaitingText = text;
            WaitingOverlayVisibility = Visibility.Visible;
        }

        private void ShowMessageOverlay( List<object> args)
        {
            
            MessageOverlayVisibility = Visibility.Visible;
            bool? result = args.FirstOrDefault() as bool?;
            if (result == null)
            {
                MessageText = args[1]?.ToString() ?? "-";
            }
            else if (result == true)
            {
                MessageText = args[2]?.ToString() ?? "-";
            }
            else
            {
                MessageText = args[3]?.ToString() ?? "-";
            }
        }

        private void HideMessageOverlay() 
        {
            MessageOverlayVisibility = Visibility.Collapsed;
        }

        private void HideAllOverlays(bool? hideProjects = true)
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
