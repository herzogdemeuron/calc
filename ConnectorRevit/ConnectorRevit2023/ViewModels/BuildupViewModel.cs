using Calc.ConnectorRevit.Helpers;
using Calc.Core.Objects;
using Calc.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calc.ConnectorRevit.Services;
using System.Diagnostics;

namespace Calc.ConnectorRevit.ViewModels
{

    public class BuildupViewModel : INotifyPropertyChanged
    {

        public void HandleBuildupSelectionChanged()
        {
            Mediator.Broadcast("BuildupSelectionChanged");
            Debug.WriteLine("BuildupSelectionChanged broadcasted");
        }

        public void HandleInherit()
        {
            Mediator.Broadcast("BuildupInherited");
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
