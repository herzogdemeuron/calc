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
        private GroupType groupType;
        private string newCode;
        private string newName;
        private string newDescription;
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
            groupType = elementSelectionSet.RevitGroupType;
            var components = ComponentCreator.CreateAssemblyComponents(elementSelectionSet.ElementIds, basicParamConfigs);
            return new ElementSourceSelectionResult()
            {
                AssemblyCode = elementSelectionSet.RevitGroupName,
                AssemblyName = elementSelectionSet.RevitGroupModel,
                Description = elementSelectionSet.RevitGroupDescription,
                Parameters = elementSelectionSet.Parameters,
                AssemblyComponents = components
            };
        }

        /// <summary>
        /// Updates the assembly group data, including the assembly basic properties and the assembly record.
        /// Serializes the assembly record and store back to revit group type.
        /// Writes record to group type parameter 'Type Comments' in a transaction.
        /// </summary>
        public void UpdateAssemblyData(string nCode, string nName, string nDescription, Unit newAssemblyUnit,AssemblyGroup newAssemblyGroup, List<AssemblyComponent> newComponents)
        {
            newCode = nCode;
            newName = nName;
            newDescription = nDescription;
            assemblyRecord = new AssemblyRecord()
            {
                AssemblyGroup = newAssemblyGroup,
                AssemblyUnit = newAssemblyUnit,
                Components = newComponents
            };
            eventHandler.Raise(StoreAssemblyData);
        }

        /// <summary>
        /// Write back the record to the type comments of the group in a transaction.
        /// </summary>
        private void StoreAssemblyData()
        {
            var recordObject = assemblyRecord.SerializeRecord();
            if (groupType == null) return;
            try
            {
                var transaction = new Transaction(doc, "Store assembly to group type: " + groupType.Name);
                transaction.Start();
                groupType.Name = newCode;
                groupType.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL).Set(newName);
                groupType.get_Parameter(BuiltInParameter.ALL_MODEL_DESCRIPTION).Set(newDescription);
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
            var recordString = groupType?.get_Parameter(BuiltInParameter.ALL_MODEL_TYPE_COMMENTS)?.AsString();
            if (recordString == null) return null;
            AssemblyRecord result = null;
            try
            {
                result = JsonConvert.DeserializeObject<AssemblyRecord>(recordString);
            }
            catch (System.Exception e)
            {
                throw new System.Exception("I don't understand the 'Type Comments' of this group.");
            }
            return result;
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
