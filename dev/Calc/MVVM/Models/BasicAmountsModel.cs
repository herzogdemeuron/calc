using Calc.Core.Objects;
using Calc.Core.Objects.BasicParameters;
using Calc.Core.Objects.Buildups;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calc.MVVM.Models
{
    /// <summary>
    /// showing the basic amounts of a ICalcComponent
    /// </summary>
    public class BasicAmountsModel
    {
        public AmountModel CountAmount { get; } = new AmountModel(Unit.piece);
        public AmountModel LengthAmount { get; } = new AmountModel(Unit.m);
        public AmountModel AreaAmount { get; } = new AmountModel(Unit.m2);
        public AmountModel VolumeAmount { get; } = new AmountModel(Unit.m3);


        /// <summary>
        /// Each time the component selection, the Buildup Unit or the Material selection is changed, the BasicUnitModel needs to be updated.
        /// </summary>
        public void UpdateAmounts(ICalcComponent host, Unit? buildupUnit, Unit? materialUnit)
        {
            CountAmount.UpdateWithHost(host, buildupUnit, materialUnit);
            LengthAmount.UpdateWithHost(host, buildupUnit, materialUnit);
            AreaAmount.UpdateWithHost(host, buildupUnit, materialUnit);
            VolumeAmount.UpdateWithHost(host, buildupUnit, materialUnit);
        }

    }
}
