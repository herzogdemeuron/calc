﻿using Calc.MVVM.Helpers.Mediators;
using Calc.Core;
using Calc.Core.Objects.GraphNodes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Calc.MVVM.ViewModels
{
    public class MappingViewModel : INotifyPropertyChanged
    {
        private CalcStore store;
        public Forest BrokenMappingForest { get; set; }
        public MappingViewModel(CalcStore calcStore)
        {
            store = calcStore;
        }

        public async Task HandleUpdateMapping()
        {
            bool? feedback;
            string error = "";
            try
            {
                MediatorToView.Broadcast("ShowWaitingOverlay", "Updating mapping...");
                feedback = await store.UpdateSelectedMapping(BrokenMappingForest);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                feedback = null;
                error = ex.Message;
            }
            MediatorToView.Broadcast("ShowMainView");
            MediatorToView.Broadcast
               ("ShowMessageOverlay",
               new List<object>
                   {   feedback,
                        error,
                        "Updated mapping successfully.",
                        "Error occured while saving, please try again."
                   }
                );
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
