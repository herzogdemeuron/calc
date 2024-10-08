using Calc.Core.Objects.Standards;
using System.ComponentModel;

namespace Calc.MVVM.Models
{
    /// <summary>
    /// The standard source of a material.
    /// </summary>
    public class StandardModel : INotifyPropertyChanged
    {
        public string Name { get; set; }
        private bool isSelected = true;
        public bool IsSelected
        {
            get => isSelected;
            set
            {
                isSelected = value;
                OnPropertyChanged(nameof(IsSelected));
            }
        }

        public StandardModel(LcaStandard standard)
        {
            Name = standard.Name;
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
