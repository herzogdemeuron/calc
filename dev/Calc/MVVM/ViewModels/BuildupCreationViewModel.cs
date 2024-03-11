using Calc.Core.Interfaces;
using Calc.Core.Objects;
using Calc.Core.Objects.Buildups;
using Calc.MVVM.Helpers.Mediators;
using System.Collections.Generic;
using System.ComponentModel;

namespace Calc.MVVM.ViewModels
{

    public class BuildupCreationViewModel : INotifyPropertyChanged
    {
        public Buildup Buildup { get; set; }
        public ICalcComponent SelectedComponent{ get; set; }
        private IBuildupComponentCreator buildupComponentCreator;

        public string CountString
        {
            get => GetValueString(Unit.piece);
        }

        public string LengthString
        {
            get => GetValueString(Unit.m);
        }

        public string AreaString
        {
            get => GetValueString(Unit.m2);
        }

        public string VolumeString
        {
            get => GetValueString(Unit.m3);
        }

        private string GetValueString(Unit unit)
        {
            return SelectedComponent.BasicParameterSet.GetAmountParam(unit).Amount?.ToString() ?? "?";
        }


        public BuildupCreationViewModel(IBuildupComponentCreator _buildupComponentCreator)
        {
            buildupComponentCreator = _buildupComponentCreator;
            //MediatorFromVM.Register("NodeItemSelectionChanged", _ => UpdateBuildupSection()); // if not, the buildup section sometimes not update,(parent reduced to zero, children remain all enabled), how to solve?
            //MediatorFromVM.Register("MappingSelectionChanged", _ => UpdateBuildupSection());
        }



        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
