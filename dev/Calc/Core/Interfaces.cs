using Calc.Core.Objects;
using Calc.Core.Objects.Buildups;
using Calc.Core.Objects.Elements;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace Calc.Core.Interfaces
{
    public interface IElementCreator
    {
        List<CalcElement> CreateCalcElements(List<CustomParamSetting> customParamSettings, List<string> parameterNameList);
    }


    public interface IElementSourceHandler
    {
        public ElementSourceSelectionResult SelectElements(List<CustomParamSetting> customParamSettings);
        public void SaveBuildupRecord(string buildupCode, string buildupName, Unit newBuildupUnit, BuildupGroup buildupGroup, string description, List<BuildupComponent> components);
        public BuildupRecord GetBuildupRecord();
    }

    public interface IImageSnapshotCreator
    {
        public string CreateImageSnapshot(string baseName);
    }

    /// <summary>
    /// send elements to speckle
    /// </summary>
    public interface IElementSender
    {
        public Task<string> SendToSpeckle(List<int> elementIds, string modelName, string buildupName,string description, Dictionary<string,string> dynamicProperties);
    }
    

    public interface IVisualizer
    {
        void ResetView(List<IGraphNode> nodes);
        void IsolateAndColorBottomBranchElements(IGraphNode node);
        void IsolateAndColorSubbranchElements(IGraphNode node);
        
    }
}
