using System;
using System.Collections.Generic;
using System.Text;

namespace Calc.Core.Objects.Materials
{
    public class MaterialComponent
    {
        public Material Material { get; set; }
        public double Ratio { get; set; } = 1;

        private Density selectedDensity;
        public Density SelectedDensity
        {
            get => selectedDensity ?? (AvailableDensities.Count > 0 ? AvailableDensities[0] : null);
        }
        public double Amount
        {
            get
            {
                if (SelectedDensity != null)
                {
                    return SelectedDensity.Quantity * Ratio;
                }
                return 0;
            }
        }
        public List<Density> AvailableDensities
        {
            get
            {
                var result = new List<Density>();
                if (Material.BulkDensity > 0)
                {
                    result.Add(new Density { Quantity = Material.BulkDensity, Unit = Unit.m3 });
                }
                if (Material.Grammage > 0)
                {
                    result.Add(new Density { Quantity = Material.Grammage, Unit = Unit.m2 });
                }
                if (Material.LinearDensity > 0)
                {
                    result.Add(new Density { Quantity = Material.LinearDensity, Unit = Unit.m });
                }
                if (Material.PieceQuantity > 0)
                {
                    result.Add(new Density { Quantity = Material.PieceQuantity, Unit = Unit.piece });
                }

                return result;
            }
        }

        private List<Unit> AvailableUnits { get => AvailableDensities.ConvertAll(x => x.Unit); }
        public string FormattedAmount { get => Math.Round(Amount, 2).ToString() + " " + SelectedDensity?.Unit??"" ; }
        public string FormattedGWP { get => Math.Round(Amount * Material.GWP, 2).ToString(); }
        public string FormattedGE { get => Math.Round(Amount * Material.GE, 2).ToString(); }
        public string FormattedCost { get => Math.Round(Amount * Material.Cost, 2).ToString(); }

        public MaterialComponent(Material material, double ratio)
        {
            Material = material;
            Ratio = ratio;
        }
    }
}
