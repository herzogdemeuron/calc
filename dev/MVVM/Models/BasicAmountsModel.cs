using Calc.Core.Objects;

namespace Calc.MVVM.Models
{
    /// <summary>
    /// Includes all basic unit amount models.
    /// </summary>
    internal class BasicAmountsModel
    {
        public AmountModel CountAmount { get; } = new AmountModel(Unit.piece);
        public AmountModel LengthAmount { get; } = new AmountModel(Unit.m);
        public AmountModel AreaAmount { get; } = new AmountModel(Unit.m2);
        public AmountModel VolumeAmount { get; } = new AmountModel(Unit.m3);

        /// <summary>
        /// Updates the basic amounts with the new host ICalcComponent and the used units.
        /// </summary>
        internal void UpdateAmounts(ICalcComponent host, Unit? assemblyUnit, Unit? materialUnit)
        {
            CountAmount.UpdateWithHost(host, assemblyUnit, materialUnit);
            LengthAmount.UpdateWithHost(host, assemblyUnit, materialUnit);
            AreaAmount.UpdateWithHost(host, assemblyUnit, materialUnit);
            VolumeAmount.UpdateWithHost(host, assemblyUnit, materialUnit);
        }

    }
}
