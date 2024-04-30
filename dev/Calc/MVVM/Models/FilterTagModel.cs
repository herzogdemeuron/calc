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
