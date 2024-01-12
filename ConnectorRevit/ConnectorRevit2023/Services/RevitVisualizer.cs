using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Calc.ConnectorRevit.Helpers.Mediators;
using Calc.ConnectorRevit.ViewModels;
using Calc.Core.Color;
using Calc.Core.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Calc.ConnectorRevit.Services
{
    [Transaction(TransactionMode.Manual)]
    public class RevitVisualizer
    {
        private Action<IGraphNode, View, ElementId> colorBranchAction;
        private NodeViewModel currentForestItem;
        private NodeViewModel selectedNodeItem;

        public RevitVisualizer()
        {
            MediatorToVisualizer.Register("TreeViewDeselected", 
                forestItem => ResetView((NodeViewModel)forestItem));
            MediatorToVisualizer.Register("AllNodesRecolored", 
                selectedItem => IsolateAndColorBottomBranchElements((NodeViewModel)selectedItem));
            MediatorToVisualizer.Register("OnBuildupItemSelectionChanged",
                selectedItem => IsolateAndColorBottomBranchElements((NodeViewModel)selectedItem));
            MediatorToVisualizer.Register("OnBranchItemSelectionChanged", 
                selectedItem => IsolateAndColorSubbranchElements((NodeViewModel)selectedItem));
        }

        private void ResetView(NodeViewModel forestItem)
        {
            currentForestItem = forestItem;
            App.EventHandler.Raise(Reset);
        }

        private void Reset()
        {
            using (Transaction t = new Transaction(App.CurrentDoc, "Reset View"))
            {
                t.Start();
                View currentView = App.CurrentDoc.ActiveView;
                currentView.TemporaryViewModes.DeactivateAllModes();
                foreach (NodeViewModel nodeItem in currentForestItem.SubNodeItems)
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

        public void IsolateAndColorSubbranchElements(NodeViewModel nodeItem)
        {
            colorBranchAction = ColorSubbranchElements;
            selectedNodeItem = nodeItem;
            App.EventHandler.Raise(IsolateAndColor);
        }

        private void IsolateAndColorBottomBranchElements(NodeViewModel nodeItem)
        {
            colorBranchAction = ColorBottomBranchElements;
            selectedNodeItem = nodeItem;
            App.EventHandler.Raise(IsolateAndColor);
        }

        public void IsolateAndColor()
        {
            if (selectedNodeItem == null) return;
            
            using (Transaction t = new Transaction(App.CurrentDoc, "Isolate and Color"))
            {
                var patternId = GetPatternId();
                IGraphNode selectedNode = selectedNodeItem.Host;
                t.Start();
                View currentView = App.CurrentDoc.ActiveView;
                IsolateElements(selectedNode, currentView);
                colorBranchAction(selectedNode, currentView, patternId);
                t.Commit();
            }
        }

        private void IsolateElements(IGraphNode node, View view)
        {
            view.TemporaryViewModes.DeactivateMode(TemporaryViewMode.TemporaryHideIsolate);
            List<string> elementIds = node.ElementIds;
            if (elementIds.Count > 0)
            {
                view.IsolateElementsTemporary(StringsToElementIds(node.ElementIds));
            }
        }

        private void ColorSubbranchElements(IGraphNode node, View view, ElementId patternId)
        {
            foreach (var subBranch in node.SubBranches)
            {
                ColorBranchElements(subBranch, view, patternId);
            }
        }

        private void ColorBottomBranchElements(IGraphNode node, View view, ElementId patternId)
        {
            ColorBranchElements(node, view, patternId);

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

        private void ColorBranchElements(IGraphNode node, View view, ElementId patternId)
        {
            HslColor hslColor = node.HslColor;
            HslColor hslColorDarker = new HslColor(hslColor.H, hslColor.S, (int)(hslColor.L * 0.6));
            RgbColor rgbColor = CalcColorConverter.HslToRgb(hslColor);
            RgbColor rgbColorDarker = CalcColorConverter.HslToRgb(hslColorDarker);
            Color color = new Color(rgbColor.R, rgbColor.G, rgbColor.B);
            Color colorDarker = new Color(rgbColorDarker.R, rgbColorDarker.G, rgbColorDarker.B);
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

        private ElementId GetPatternId()
        {
            FilteredElementCollector collector = new FilteredElementCollector(App.CurrentDoc);
            ICollection<Element> patterns = collector.OfClass(typeof(FillPatternElement)).ToElements();
            return patterns.FirstOrDefault().Id;
        }
        private List<ElementId> StringsToElementIds(List<string> elememtIdStrings)
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
