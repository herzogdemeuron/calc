﻿using System;
using System.Collections.Generic;
using System.Text;
using Calc.Core.Objects.BasicParameters;
using Calc.Core.Objects.Materials;

namespace Calc.Core.Objects
{
    /// <summary>
    /// A layer or component of an element type in revit/rhino.
    /// A LayerComponent must have mapped infos，could have Ids to be targeted.
    /// </summary>
    public class LayerComponent
    {
        public string TargetMaterialName { get; }
        public MaterialComponentSet MaterialComponentSet { get; set; }
        private BasicParameterSet basicParameterSet;

        public LayerComponent(string targetMaterialName, BasicParameterSet basicParameterSet = null)
        {
            TargetMaterialName = targetMaterialName;
            this.basicParameterSet = basicParameterSet;
        }

        public bool CheckTarget( LayerComponent layerComponent)
        {
            return TargetMaterialName == layerComponent.TargetMaterialName;
        }

        public void ApplyTarget(LayerComponent layerComponent)
        {
            this.MaterialComponentSet = layerComponent.MaterialComponentSet;
        }

        public void ResetTarget() // is this needed?
        {
            this.basicParameterSet = null;
        }

        public void SetMainMaterial(Material material)
        {
            MaterialComponentSet.SetMainMaterial(material);
        }

        public void SetSubMaterial(Material material)
        {
            MaterialComponentSet.SetSubMaterial(material);
        }

        public BasicParameter GetQuantity(Unit unit)
        {
            if (basicParameterSet == null) return null;
            return basicParameterSet.GetQuantity(unit);
        }

    }
}
