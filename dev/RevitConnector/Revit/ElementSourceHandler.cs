using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Calc.Core;
using Calc.Core.Objects;
using Calc.Core.Objects.Assemblies;
using Calc.Core.Objects.Elements;
using Calc.RevitConnector.Config;
using Calc.RevitConnector.Helpers;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Calc.RevitConnector.Revit
{
    /// <summary>
    /// Gets assembly components from revit elements.
    /// Gets and stores the assembly record.
    /// </summary>
    public class ElementSourceHandler : IElementSourceHandler
    {
        private readonly UIDocument uidoc;
        private readonly Document doc;
        private readonly RevitExternalEventHandler eventHandler;
        private int groupTypeId;
        private string newCode;
        private AssemblyRecord assemblyRecord;
        public AssemblyComponentCreator ComponentCreator { get; }

        public ElementSourceHandler(UIDocument udoc, RevitExternalEventHandler eHandler)
        {
            uidoc = udoc;
            doc = uidoc.Document;
            eventHandler = eHandler;
            ComponentCreator = new AssemblyComponentCreator(uidoc);
        }

        /// <summary>
        /// Selects elements in revit, creates the raw selection result.
        /// </summary>
        public ElementSourceSelectionResult SelectElements(List<CustomParamSetting> customParamSettings)
        {
            var basicParamConfigs = GetParamSettings(customParamSettings);
            var elementSelectionSet = SelectionHelper.SelectElements(uidoc);
            groupTypeId = elementSelectionSet.RevitGroupTypeId;
            var components = ComponentCreator.CreateAssemblyComponents(elementSelectionSet.ElementIds, basicParamConfigs);
            return new ElementSourceSelectionResult()
            {
                AssemblyCode = elementSelectionSet.RevitGroupName,
                Parameters = elementSelectionSet.Parameters,
                AssemblyComponents = components
            };
        }

        /// <summary>
        /// Serializes the assembly record and store back to revit group type.
        /// Writes record to group type parameter 'Type Comments' in a transaction.
        /// </summary>
        public void SaveAssemblyRecord(string nCode, string newName, Unit newAssemblyUnit,AssemblyGroup newAssemblyGroup, string newDescription, List<AssemblyComponent> newComponents)
        {
            newCode = nCode;
            assemblyRecord = new AssemblyRecord()
            {
                AssemblyName = newName,
                AssemblyGroup = newAssemblyGroup,
                AssemblyUnit = newAssemblyUnit,
                Description = newDescription,
                Components = newComponents
            };
            eventHandler.Raise(StoreAssemblyRecord);
        }

        private void StoreAssemblyRecord()
        {
            var recordObject = assemblyRecord.SerializeRecord();
            var groupType = doc.GetElement(new ElementId(groupTypeId)) as GroupType;
            try
            {
                var transaction = new Transaction(doc, "Store assembly to group type: " + groupType.Name);
                transaction.Start();
                groupType.Name = newCode;
                var jsonString = JsonConvert.SerializeObject(recordObject);
                groupType.get_Parameter(BuiltInParameter.ALL_MODEL_TYPE_COMMENTS).Set(jsonString);
                transaction.Commit();
            }
            catch(System.Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Gets the assembly record from the group type parameter 'Type Comments'.
        /// </summary>
        public AssemblyRecord GetAssemblyRecord()
        {
            var groupType = doc.GetElement(new ElementId(groupTypeId)) as GroupType;
            var recordString = groupType?.get_Parameter(BuiltInParameter.ALL_MODEL_TYPE_COMMENTS)?.AsString();
            if (recordString == null) return null;
            try
            {
                return JsonConvert.DeserializeObject<AssemblyRecord>(recordString);
            }
            catch(JsonSerializationException e)
            {
                return null;
            }
        }

        private List<RevitBasicParamConfig> GetParamSettings(List<CustomParamSetting> customParamSettings)
        {
            var basicParamConfigs = new List<RevitBasicParamConfig>();
            foreach (CustomParamSetting paramSetting in customParamSettings)
            {
                var setting = ParameterHelper.ParseFromParamSetting(paramSetting);
                if (setting != null) basicParamConfigs.Add(setting);
            }
            return basicParamConfigs;
        }

    }
}
