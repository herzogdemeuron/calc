using Calc.Core.Calculation;
using Calc.Core.Objects.Buildups;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Calc.Core.Objects.Materials
{
    public class MaterialSet // deprecated
    {
        public Material MainMaterial { get; set; }
        public Material SubMaterial { get; set; }
        public MaterialFunction Function { get; set; }
        public double SubMaterialRatio { get; set; } = 0;
        public double MainMaterialRatio { get => 1 - SubMaterialRatio; }

        public bool HasMainMaterial { get => MainMaterial != null; }
        public bool HasSubMaterial { get => HasMainMaterial && SubMaterial != null; }

        public double GetMaterialGwp(bool getMain = true)
        {
            var hasMaterial = getMain ? HasMainMaterial : HasSubMaterial;
            if (hasMaterial)
            {
                var material = getMain ? MainMaterial : SubMaterial;
                var ratio = getMain ? MainMaterialRatio : SubMaterialRatio;
                var gwp = material.Gwp ?? 0;
                return gwp * ratio;
            };
            return 0;
        }

        public double GetMaterialGe(bool getMain = true)
        {
            var hasMaterial = getMain ? HasMainMaterial : HasSubMaterial;
            if (hasMaterial)
            {
                var material = getMain ? MainMaterial : SubMaterial;
                var ratio = getMain ? MainMaterialRatio : SubMaterialRatio;
                var ge = material.Ge ?? 0;
                return ge * ratio;
            };
            return 0;
        }

    }
}
