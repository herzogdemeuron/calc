using Autodesk.Revit.UI;
using Calc.Core.Interfaces;
using Calc.Core.Objects.Elements;
using Calc.RevitConnector.Config;
using System;
using System.Collections.Generic;
using Calc.RevitConnector.Helpers;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calc.RevitConnector.Revit
{
    public class ElementSourceHandler : IElementSourceHandler
    {
        public BuildupComponentCreator ComponentCreator { get; }
        private UIDocument Uidoc { get; }

        public ElementSourceHandler(UIDocument uidoc)
        {
            Uidoc = uidoc;
            ComponentCreator = new BuildupComponentCreator(uidoc);
        }

        public ElementSourceSelectionResult SelectElements(List<CustomParamSetting> customParamSettings)
        {
            var basicParamConfigs = new List<RevitBasicParamConfig>();
            foreach (CustomParamSetting paramSetting in customParamSettings)
            {
                var setting = ParameterHelper.ParseFromParamSetting(paramSetting);
                if (setting != null) basicParamConfigs.Add(setting);
            }

            var elementSelectionSet = SelectionHelper.SelectElements(Uidoc);
            var ids = elementSelectionSet.ElementIds;
            var components = ComponentCreator.CreateBuildupComponents(ids, basicParamConfigs);
            return new ElementSourceSelectionResult()
            {
                GroupName = elementSelectionSet.GroupName,
                Parameters = elementSelectionSet.Parameters,
                BuildupComponents = components
            };
        }

    }
}
