using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Calc.Core.Color;
using Calc.Core.Objects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Calc.ConnectorRevit
{
    [Transaction(TransactionMode.Manual)]
    public static class Visualizer
    {
        public static void Reset()
        {
            Debug.WriteLine("ShowAll");
            using (Transaction t = new Transaction(App.CurrentDoc, "ShowAll"))
            {
                t.Start();

                var currentView = App.CurrentDoc.ActiveView;
                currentView.TemporaryViewModes.DeactivateAllModes();
                var overrideGraphicSettings = new OverrideGraphicSettings();

                foreach (var branchItem in App.ViewModel.BranchItems)
                {
                    Tree tree = branchItem.Branch as Tree;
                    var elements = StringsToElementIds(tree.ElementIds);
                    foreach (var element in elements)
                    {
                        currentView.SetElementOverrides(element, overrideGraphicSettings);
                    }
                }

                t.Commit();
            }
        }

        public static void IsolateAndColor()
        {
            using (Transaction t = new Transaction(App.CurrentDoc, "Isolate and Color"))
            {
                FilteredElementCollector collector = new FilteredElementCollector(App.CurrentDoc);
                ICollection<Element> patterns = collector.OfClass(typeof(FillPatternElement)).ToElements();
                var patternId = patterns.FirstOrDefault().Id;
                Branch selectedBranch = App.ViewModel.SelectedBranchItem.Branch;
                t.Start();

                var currentView = App.CurrentDoc.ActiveView;
                IsolateElements(selectedBranch, currentView);
                BranchPainter.ColorBranchesByBranch(selectedBranch.SubBranches);
                ColorElements(selectedBranch, currentView, patternId);

                t.Commit();
            }
        }
        private static void IsolateElements(Branch branch, View view)
        {
            view.TemporaryViewModes.DeactivateMode(TemporaryViewMode.TemporaryHideIsolate);
            view.IsolateElementsTemporary(StringsToElementIds(branch.ElementIds));
        }

        private static void ColorElements(Branch branch, View view, ElementId patternId)
        {
            foreach (var subBranch in branch.SubBranches)
            {
                var hslColorDarker = new HslColor(
                    subBranch.HslColor.Hue,
                    subBranch.HslColor.Saturation,
                    (int)(subBranch.HslColor.Lightness * 0.6));

                var rgbColor = ColorConverter.HslToRgb(subBranch.HslColor);
                var rgbColorDarker = ColorConverter.HslToRgb(hslColorDarker);

                var color = new Color(rgbColor.Red, rgbColor.Green, rgbColor.Blue);
                var colorDarker = new Color(rgbColorDarker.Red, rgbColorDarker.Green, rgbColorDarker.Blue);

                var overrideGraphicSettings = new OverrideGraphicSettings();
                overrideGraphicSettings.SetProjectionLineColor(colorDarker);
                overrideGraphicSettings.SetSurfaceTransparency(0);

                overrideGraphicSettings.SetSurfaceForegroundPatternId(patternId);
                overrideGraphicSettings.SetSurfaceForegroundPatternColor(color);

                foreach (var element in StringsToElementIds(subBranch.ElementIds))
                {
                    view.SetElementOverrides(element, overrideGraphicSettings);
                }
            }
        }

        private static List<ElementId> StringsToElementIds(List<string> elememtIdStrings)
        {             
            List<ElementId> elementIds = new List<ElementId>();
                   
            foreach (string elementIdString in elememtIdStrings)
            {
                elementIds.Add(new ElementId(Convert.ToInt32(elementIdString)));
            }
            return elementIds;
        }
    }
}
