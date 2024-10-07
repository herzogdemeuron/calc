using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Calc.MVVM.Models
{
    /// <summary>
    /// Used by the calc builder.
    /// A filter tag for material selection.
    /// </summary>
    internal class FilterTagModel: INotifyPropertyChanged
    {
        public string TagName { get; set; }
        private bool isSelected = false;
        public bool IsSelected
        {
            get => isSelected;
            set
            {
                isSelected = value;
                OnPropertyChanged(nameof(IsSelected));
            }
        }
        public bool HasCount => DynamicCount != "0";
        private string dynamicCount;
        public string DynamicCount
        {
            get => dynamicCount;
            set
            {
                dynamicCount = value;
                OnPropertyChanged(nameof(DynamicCount));
                OnPropertyChanged(nameof(HasCount));
            }
        }
        private Dictionary<FilterTagModel,int> relationCount = new Dictionary<FilterTagModel, int>();

        public FilterTagModel(string name)
        {
            TagName = name;
        }

        internal void AddRelationCount(FilterTagModel tagModel)
        {
            if (relationCount.ContainsKey(tagModel))
            {
                relationCount[tagModel]++;
            }
            else
            {
                relationCount.Add(tagModel, 1);
            }
        }

        /// <summary>
        /// Shows the count of the tagmodel in the dynamic count.
        /// </summary>
        internal void UpdateDynamicCount(FilterTagModel tagModel)
        {

            if(tagModel == null)
            {
                DynamicCount = relationCount.Values.Sum().ToString();
            }
            else if (relationCount.ContainsKey(tagModel))
            {
                DynamicCount = relationCount[tagModel].ToString();
            }
            else
            {
                DynamicCount = "0";
            }
        }

        internal void CleanDynamicCount()
        {
            DynamicCount = "";
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
