using Calc.Core.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Calc.Core.Interfaces
{
    public interface ElementCreator
    {
        List<CalcElement> CreateCalcElements(List<string> parameterNameList);
    }

    public interface Visualizer
    {
        
    }
}
