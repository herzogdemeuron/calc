using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calc.MVVM.Models
{
    public class FilterTagModel: INotifyPropertyChanged
    {
        public string Name { get; set; }
        public string Family { get; set; }
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

        private string dynamicCount;
        public string DynamicCount
        {
            get => dynamicCount;
            set
            {
                dynamicCount = value;
                OnPropertyChanged(nameof(DynamicCount));
            }
        }

        private Dictionary<FilterTagModel,int> relationCount = new Dictionary<FilterTagModel, int>();

        public FilterTagModel(string name, string family)
        {
            Name = name;
            Family = family;
        }

        public void AddRelationCount(FilterTagModel tagModel)
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
        /// show the count of the tagmodel in the dynamic count
        /// </summary>
        public void UpdateDynamicCount(FilterTagModel tagModel)
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

        public void CleanDynamicCount()
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
