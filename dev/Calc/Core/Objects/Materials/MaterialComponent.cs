using System;
using System.Collections.Generic;
using System.Text;

namespace Calc.Core.Objects.Materials
{
    public class MaterialComponent // deprecated
    {
        public Material Material { get; set; }
        public double Ratio { get; set; } = 1;
        public Density Density { get; set; }
        public string Function { get; set; }
        public double QuantityPerUnit
        {
            get
            {
                if (Density != null)
                {
                    return Density.Quantity * Ratio;
                }
                return 0;
            }
        }

        public MaterialComponent(Material material, double ratio)
        {
            Material = material;
            Ratio = ratio;
        }
    }
}
