using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Calc.Core.Interfaces;
using Calc.Core.Objects.Buildups;
using Calc.Core.Objects.Elements;
using Calc.RevitConnector.Config;
using Calc.RevitConnector.Helpers;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Calc.RevitConnector.Revit
{
    /// <summary>
    /// get buildup components from revit elements,
    /// get and store the buildup record
    /// </summary>
    public class ElementSourceHandler : IElementSourceHandler
    {
        private UIDocument uidoc;
        private readonly Document doc;
        private readonly RevitExternalEventHandler eventHandler;
        private int groupTypeId;
        private string newCode;
        private BuildupRecord buildupRecord;
        public BuildupComponentCreator ComponentCreator { get; }

        public ElementSourceHandler(UIDocument udoc, RevitExternalEventHandler eHandler)
        {
            uidoc = udoc;
            doc = uidoc.Document;
            eventHandler = eHandler;
            ComponentCreator = new BuildupComponentCreator(uidoc);
        }

        /// <summary>
        /// select elements in revit, get the raw selection result
        /// </summary>
        /// <param name="customParamSettings"></param>
        /// <returns></returns>
        public ElementSourceSelectionResult SelectElements(List<CustomParamSetting> customParamSettings)
        {
            var basicParamConfigs = GetParamSettings(customParamSettings);
            var elementSelectionSet = SelectionHelper.SelectElements(uidoc);
            groupTypeId = elementSelectionSet.RevitGroupTypeId;
            var components = ComponentCreator.CreateBuildupComponents(elementSelectionSet.ElementIds, basicParamConfigs);
            return new ElementSourceSelectionResult()
            {
                BuildupCode = elementSelectionSet.RevitGroupName,
                Parameters = elementSelectionSet.Parameters,
                BuildupComponents = components
            };
        }

        /// <summary>
        /// serialize the buildup record and store back to revit group type
        /// write record to group type parameter 'Type Comments' in a transaction
        /// </summary>
        public void SaveBuildupRecord(string nCode, string newName, BuildupGroup newBuildupGroup, string newDescription, List<BuildupComponent> newComponents)
        {
            newCode = nCode;
            buildupRecord = new BuildupRecord()
            {
                BuildupName = newName,
                BuildupGroup = newBuildupGroup,
                Description = newDescription,
                Components = newComponents
            };
            eventHandler.Raise(StoreBuildupRecord);
        }

        public void StoreBuildupRecord()
        {
            var recordObject = buildupRecord.SerializeRecord();
            var groupType = doc.GetElement(new ElementId(groupTypeId)) as GroupType;
            try
            {
                var transaction = new Transaction(doc, "Store buildup to group type: " + groupType.Name);
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
        /// get the buildup record from the group type parameter 'Type Comments'
        /// </summary>
        public BuildupRecord GetBuildupRecord()
        {
            var groupType = doc.GetElement(new ElementId(groupTypeId)) as GroupType;
            var recordString = groupType?.get_Parameter(BuiltInParameter.ALL_MODEL_TYPE_COMMENTS)?.AsString();
            if (recordString == null) return null;

            return JsonConvert.DeserializeObject<BuildupRecord>(recordString);
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
