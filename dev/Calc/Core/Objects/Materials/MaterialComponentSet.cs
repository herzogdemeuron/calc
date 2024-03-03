using Speckle.Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Calc.Core.Objects.Materials
{
    public class MaterialComponentSet
    {
        public MaterialComponent MainMaterialComponent { get; set; }
        public MaterialComponent SubMaterialComponent { get; set; }

        public bool IsMapped { get => MainMaterialComponent != null; }

        public void SetMainMaterial(Material material)
        {
            MainMaterialComponent = SetMaterialComponent(MainMaterialComponent, material, 1);
        }

        public void SetSubMaterial(Material material)
        {
            SubMaterialComponent = SetMaterialComponent(SubMaterialComponent, material, 0);
        }

        private MaterialComponent SetMaterialComponent(MaterialComponent materialComponent, Material material, double ratio)
        {
            if (materialComponent == null)
            {
                return new MaterialComponent(material, ratio);
            }
            else
            {
                materialComponent.Material = material;
                //materialComponent.Ratio = ratio;
                return materialComponent;
            }
        }

        public void SetSubMaterialRatio(double ratio)
        {
            if (MainMaterialComponent == null) return;

            if (ratio < 0 || ratio >= 1) return;

            if (SubMaterialComponent != null)
            {
                SubMaterialComponent.Ratio = ratio;
                MainMaterialComponent.Ratio = 1 - ratio;
            }
        }
    }
}
