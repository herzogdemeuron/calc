using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Calc.Core.Objects.BasicParameters;
using Calc.Core.Objects.Materials;

namespace Calc.Core.Objects.Buildups
{
    /// <summary>
    /// A layer or component of an element type in revit/rhino.
    /// A LayerComponent must have mapped infos，could have Ids to be targeted.
    /// </summary>
    public class LayerComponent : INotifyPropertyChanged, ICalcComponent
    {
        public string TargetMaterialName { get; }
        public double? Thickness { get; set; }
        public MaterialComponentSet MaterialComponentSet { get; set; }
        public BasicParameterSet BasicParameterSet { get; set; }
        public LayerComponent(string targetMaterialName, BasicParameterSet basicParameterSet = null, double? thickness = null)
        {
            TargetMaterialName = targetMaterialName;
            BasicParameterSet = basicParameterSet;
            Thickness = thickness;
        }

        

/*        public bool CheckSource(LayerComponent layerComponent)
        {
            return TargetMaterialName == layerComponent.TargetMaterialName;
        }

        public void ApplySource(LayerComponent layerComponent)
        {
            MaterialComponentSet = layerComponent.MaterialComponentSet;
        }
*/
        public void ResetTarget() // is this needed?
        {
            BasicParameterSet = null;
        }

        public void SetMainMaterial(Material material)
        {
            MaterialComponentSet.SetMainMaterial(material);
        }

        public void SetSubMaterial(Material material)
        {
            MaterialComponentSet.SetSubMaterial(material);
        }

        public BasicParameter GetAmountParam(Unit unit)
        {
            if (BasicParameterSet == null) return null;
            return BasicParameterSet.GetAmountParam(unit);
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
