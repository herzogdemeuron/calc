﻿using Calc.ConnectorRevit.Helpers;
using Calc.Core.Objects;
using System.Collections.Generic;
using System.ComponentModel;

namespace Calc.ConnectorRevit.ViewModels
{

    public class BuildupViewModel : INotifyPropertyChanged
    {
        private NodeTreeViewModel nodeTreeVM;
        public BuildupViewModel(NodeTreeViewModel ntVM)
        {
            nodeTreeVM = ntVM;
        }

        public void HandleBuildupSelectionChanged(int index, Buildup newBuildups)
        {
            var branch = nodeTreeVM.SelectedNodeItem?.Host as Branch;
            var buildups = new List<Buildup>(branch.Buildups)
            {
                [index] = newBuildups
            };
            branch.Buildups = buildups;

            Mediator.Broadcast("BuildupSelectionChanged");
        }

        public void HandleInherit()
        {
            if (nodeTreeVM.SelectedNodeItem == null)
                return;
            IGraphNode host = nodeTreeVM.SelectedNodeItem.Host;
            if (!(host is Branch branch))
                return;
            branch.InheritMapping();
            Mediator.Broadcast("BuildupSelectionChanged");
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
