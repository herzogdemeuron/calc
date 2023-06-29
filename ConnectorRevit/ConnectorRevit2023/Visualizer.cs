using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Calc.ConnectorRevit.Views;
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
        private static Action<Branch, View, ElementId> colorBranchAction;

        public static void Reset()
        {
            using (Transaction t = new Transaction(App.CurrentDoc, "Reset View"))
            {
                t.Start();
                View currentView = App.CurrentDoc.ActiveView;
                currentView.TemporaryViewModes.DeactivateAllModes();
                foreach (BranchViewModel branchItem in App.ViewModel.BranchItems)
                {
                    List<ElementId> elementIds = StringsToElementIds(branchItem.Branch.ElementIds);
                    foreach (ElementId elementId in elementIds)
                    {
                        currentView.SetElementOverrides(elementId, new OverrideGraphicSettings());
                    }
                }
                t.Commit();
            }
        }

        public static void IsolateAndColorSubbranchElements()
        {
            colorBranchAction = ColorSubbranchElements;
            IsolateAndColor();
        }

        public static void IsolateAndColorBottomBranchElements()
        {
            colorBranchAction = ColorBottomBranchElements;
            IsolateAndColor();
        }

        public static void IsolateAndColor()
        {
            using (Transaction t = new Transaction(App.CurrentDoc, "Isolate and Color"))
            {
                var patternId = GetPatternId();
                Branch selectedBranch = App.ViewModel.SelectedBranchItem.Branch;
                t.Start();
                View currentView = App.CurrentDoc.ActiveView;
                IsolateElements(selectedBranch, currentView);
                colorBranchAction(selectedBranch, currentView, patternId);
                t.Commit();
            }
        }

        private static void IsolateElements(Branch branch, View view)
        {
            view.TemporaryViewModes.DeactivateMode(TemporaryViewMode.TemporaryHideIsolate);
            view.IsolateElementsTemporary(StringsToElementIds(branch.ElementIds));
        }


        private static void ColorSubbranchElements(Branch branch, View view, ElementId patternId)
        {
            foreach (var subBranch in branch.SubBranches)
            {
                ColorBranchElements(subBranch, view, patternId);
            }
        }

        private static void ColorBottomBranchElements(Branch branch, View view, ElementId patternId)
        {
            foreach (var subBranch in branch.SubBranches)
            {
                if (subBranch.SubBranches.Count == 0)
                {
                    ColorBranchElements(subBranch, view, patternId);
                }
                else
                {
                    ColorBottomBranchElements(subBranch, view, patternId);
                }
            }
        }

        private static void ColorBranchElements(Branch branch, View view, ElementId patternId)
        {
            HslColor hslColor = branch.HslColor;
            HslColor hslColorDarker = new HslColor(hslColor.Hue, hslColor.Saturation, (int)(hslColor.Lightness * 0.6));
            RgbColor rgbColor = CalcColorConverter.HslToRgb(hslColor);
            RgbColor rgbColorDarker = CalcColorConverter.HslToRgb(hslColorDarker);
            Color color = new Color(rgbColor.Red, rgbColor.Green, rgbColor.Blue);
            Color colorDarker = new Color(rgbColorDarker.Red, rgbColorDarker.Green, rgbColorDarker.Blue);
            var overrideSettings = new OverrideGraphicSettings();
            overrideSettings.SetProjectionLineColor(colorDarker);
            overrideSettings.SetSurfaceTransparency(0);
            overrideSettings.SetSurfaceForegroundPatternId(patternId);
            overrideSettings.SetSurfaceForegroundPatternColor(color);

            foreach (ElementId elementId in StringsToElementIds(branch.ElementIds))
            {
                view.SetElementOverrides(elementId, overrideSettings);
            }

        }

        private static ElementId GetPatternId()
        {
            FilteredElementCollector collector = new FilteredElementCollector(App.CurrentDoc);
            ICollection<Element> patterns = collector.OfClass(typeof(FillPatternElement)).ToElements();
            return patterns.FirstOrDefault().Id;
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
