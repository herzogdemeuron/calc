using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Calc.Core.Color;
using Calc.Core.Interfaces;
using Calc.Core.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Calc.RevitConnector.Revit
{
    [Transaction(TransactionMode.Manual)]
    public class RevitVisualizer : IVisualizer
    {
        private readonly RevitExternalEventHandler eventHandler;
        private readonly Document doc;
        private Action<IGraphNode, View, ElementId> colorBranchAction;
        private List<IGraphNode> currentNodes = new List<IGraphNode>();
        private IGraphNode selectedNode;

        public RevitVisualizer(Document doc, RevitExternalEventHandler eventHandler)
        {
            this.eventHandler = eventHandler;
            this.doc = doc;
        }

        public void ResetView(List<IGraphNode> nodes)
        {
            currentNodes = nodes;
            eventHandler.Raise(Reset);
        }

        private void Reset()
        {
            using (Transaction t = new Transaction(doc, "Reset View"))
            {
                t.Start();
                View currentView = doc.ActiveView;
                currentView.TemporaryViewModes.DeactivateAllModes();
                foreach (IGraphNode node in currentNodes)
                {
                    List<ElementId> elementIds = StringsToElementIds(node.ElementIds);
                    foreach (ElementId elementId in elementIds)
                    {
                        currentView.SetElementOverrides(elementId, new OverrideGraphicSettings());
                    }
                }
                t.Commit();
            }
        }

        public void IsolateAndColorSubbranchElements(IGraphNode node)
        {
            if (node == null) return;
            colorBranchAction = ColorSubbranchElements;
            selectedNode = node;
            eventHandler.Raise(IsolateAndColor);
        }

        public void IsolateAndColorBottomBranchElements(IGraphNode node)
        {
            if (node == null) return;
            colorBranchAction = ColorBottomBranchElements;
            selectedNode = node;
            eventHandler.Raise(IsolateAndColor);
        }

        public void IsolateAndColor()
        {
            if (selectedNode == null) return;

            using (Transaction t = new Transaction(doc, "Isolate and Color"))
            {
                var patternId = GetPatternId();
                t.Start();
                View currentView = doc.ActiveView;
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
            FilteredElementCollector collector = new FilteredElementCollector(doc);
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
