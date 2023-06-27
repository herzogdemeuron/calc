using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using Calc.Core.Color;
using Calc.Core.Objects;

namespace Calc.ConnectorRevit
{
    [Transaction(TransactionMode.Manual)]
    public static class Visualizer
    {
        private static Branch SelectedBranch
        {
            get
            {
                return App.ViewModel.SelectedBranchItem.Branch;
            }
        }

        private static Document Doc
        {
            get
            {
                return App.CurrentDoc;
            }
        }

        public static void Reset()
        {
            Debug.WriteLine("ShowAll");
            using (Transaction t = new Transaction(Doc, "ShowAll"))
            {
                t.Start();

                var currentView = Doc.ActiveView;
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
            using (Transaction t = new Transaction(Doc, "Isolate and Color"))
            {
                FilteredElementCollector collector = new FilteredElementCollector(Doc);
                ICollection<Element> patterns = collector.OfClass(typeof(FillPatternElement)).ToElements();
                var patternId = patterns.FirstOrDefault().Id;

                t.Start();

                var currentView = Doc.ActiveView;
                IsolateElements(SelectedBranch, currentView);
                BranchPainter.ColorBranchesByBranch(SelectedBranch.SubBranches);
                ColorElements(SelectedBranch, currentView, patternId);

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
