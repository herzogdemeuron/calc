using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Calc.ConnectorRevit.ViewModels;
using Calc.Core.Color;
using Calc.Core.Objects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Calc.ConnectorRevit.Services
{
    [Transaction(TransactionMode.Manual)]
    public static class Visualizer
    {
        private static Action<IGraphNode, View, ElementId> colorBranchAction;

        public static void Reset()
        {
            using (Transaction t = new Transaction(App.CurrentDoc, "Reset View"))
            {
                t.Start();
                View currentView = App.CurrentDoc.ActiveView;
                currentView.TemporaryViewModes.DeactivateAllModes();
                foreach (NodeViewModel nodeItem in App.ViewModel.CurrentForestItem.SubNodeItems)
                {
                    List<ElementId> elementIds = StringsToElementIds(nodeItem.Host.ElementIds);
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
                IGraphNode selectedNode = App.ViewModel.SelectedNodeItem.Host;
                t.Start();
                View currentView = App.CurrentDoc.ActiveView;
                IsolateElements(selectedNode, currentView);
                colorBranchAction(selectedNode, currentView, patternId);
                t.Commit();
            }
        }

        private static void IsolateElements(IGraphNode node, View view)
        {
            view.TemporaryViewModes.DeactivateMode(TemporaryViewMode.TemporaryHideIsolate);
            List<string> elementIds = node.ElementIds;
            if (elementIds.Count > 0)
            {
                view.IsolateElementsTemporary(StringsToElementIds(node.ElementIds));
            }
        }


        private static void ColorSubbranchElements(IGraphNode node, View view, ElementId patternId)
        {
            ColorBranchElements(node, view, patternId);

            foreach (var subBranch in node.SubBranches)
            {
                ColorBranchElements(subBranch, view, patternId);
            }
        }

        private static void ColorBottomBranchElements(IGraphNode node, View view, ElementId patternId)
        {
            if (node.SubBranches.Count == 0)
            {
                ColorBranchElements(node, view, patternId);
            }

            foreach (var subBranch in node.SubBranches)
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

        private static void ColorBranchElements(IGraphNode node, View view, ElementId patternId)
        {
            HslColor hslColor = node.HslColor;
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

            foreach (ElementId elementId in StringsToElementIds(node.ElementIds))
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
