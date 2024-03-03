using System;
using System.Collections.Generic;
using System.Text;
using Calc.Core.Objects.BasicParameters;

namespace Calc.Core.Objects
{
    /// <summary>
    /// A layer or component of an element type in revit/rhino.
    /// A LayerComponent must have mapped infos，could have Ids to be targeted.
    /// </summary>
    public class LayerComponent
    {
        public List<int> Ids { get; set; }
        public string TargetTypeName { get; }
        public string TargetMaterialName { get; }
        public int Count {  get => Ids?.Count??0;}
        private BasicParameterSet basicParameterSet;

        public LayerComponent(string targetTypeName, string targetMaterialName)
        {
            TargetTypeName = targetTypeName;
            TargetMaterialName = targetMaterialName;            
        }

        public bool CheckTarget( LayerComponent layerComponent)
        {
            return TargetTypeName == layerComponent.TargetTypeName && TargetMaterialName == layerComponent.TargetMaterialName;
        }

        public void SetTarget(LayerComponent layerComponent)
        {
            this.Ids = layerComponent.Ids;
            this.basicParameterSet = layerComponent.basicParameterSet;
        }

        public void ResetTarget()
        {
            this.Ids = null;
            this.basicParameterSet = null;
        }

        public BasicParameter GetQuantity(Unit unit)
        {
            return basicParameterSet.GetQuantity(unit);
        }

    }
}
