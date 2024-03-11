using Calc.Core.Objects;
using Calc.Core.Objects.Buildups;
using System;
using System.Collections.Generic;
using System.Text;

namespace Calc.Core.Interfaces
{
    public interface IElementCreator
    {
        List<CalcElement> CreateCalcElements(List<string> parameterNameList);
    }

    public interface IBuildupComponentCreator
    {
        public List<BuildupComponent> CreateBuildupComponentsFromSelection()
    }

    public interface IVisualizer
    {
        void ResetView(List<IGraphNode> nodes);
        void IsolateAndColorBottomBranchElements(IGraphNode node);
        void IsolateAndColorSubbranchElements(IGraphNode node);
        
    }
}
