using Calc.Core.Objects;
using Calc.Core.Objects.Assemblies;
using Calc.Core.Objects.Elements;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Calc.Core
{
    /// <summary>
    /// Creates calc elements in the soruce app.
    /// </summary>
    public interface IElementCreator
    {
        List<CalcElement> CreateCalcElements(List<CustomParamSetting> customParamSettings, List<string> parameterNameList);
    }

    /// <summary>
    /// Used in calc builder.
    /// Selects elements in the source app.
    /// </summary>
    public interface IElementSourceHandler
    {
        public ElementSourceSelectionResult SelectElements(List<CustomParamSetting> customParamSettings);
        public void SaveAssemblyRecord(string assemblyCode, string assemblyName, Unit newAssemblyUnit, AssemblyGroup assemblyGroup, string description, List<AssemblyComponent> components);
        public AssemblyRecord GetAssemblyRecord();
    }

    /// <summary>
    /// Creates screenshots in the source app.
    /// </summary>
    public interface IImageSnapshotCreator
    {
        public string CreateImageSnapshot(string baseName);
    }

    /// <summary>
    /// Sends elements to speckle.
    /// </summary>
    public interface IElementSender
    {
        public Task<string> SendAssembly(AssemblyData assemblyData);
    }    

    /// <summary>
    /// Visualizes elements in the source app.
    /// </summary>
    public interface IVisualizer
    {
        void ResetView(List<IGraphNode> nodes);
        void IsolateAndColorBottomBranchElements(IGraphNode node);
        void IsolateAndColorSubbranchElements(IGraphNode node);
        
    }
}
