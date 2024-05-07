using System;
using System.Collections.Generic;
using System.Text;

namespace Calc.Core.Objects.Materials
{
    public class Density // deprecated
    {
        public Unit Unit { get; set; }
        public double Quantity { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is Density other)
            {
                return other.Unit == Unit && other.Quantity == Quantity;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Unit.GetHashCode() ^ Quantity.GetHashCode(); 
        }
    }
}
