using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Calc.Core.Color;
using Calc.Core;
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
        private Action<IGraphNode, View, ElementId> colorizeBranchAction;
        private List<IGraphNode> currentNodes = new List<IGraphNode>();
        private IGraphNode selectedNode;

        public RevitVisualizer(Document doc, RevitExternalEventHandler eventHandler)
        {
            this.eventHandler = eventHandler;
            this.doc = doc;
        }

        /// <summary>
        /// Resets graphic overrides on the elements of the nodes.
        /// </summary>
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

                currentView.TemporaryViewModes?.DeactivateAllModes();
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

        public void IsolateAndColorizeSubbranchElements(IGraphNode node)
        {
            colorizeBranchAction = ColorizeSubbranchElements;
            selectedNode = node;
            eventHandler.Raise(IsolateAndColorize);
        }

        public void IsolateAndColorizeBottomBranchElements(IGraphNode node)
        {
            colorizeBranchAction = ColorizeBottomBranchElements;
            selectedNode = node;
            eventHandler.Raise(IsolateAndColorize);
        }

        public void IsolateAndColorize()
        {
            if (selectedNode == null) return;

            using (Transaction t = new Transaction(doc, "Isolate and Color"))
            {
                var patternId = GetPatternId();
                t.Start();
                View currentView = doc.ActiveView;
                IsolateElements(selectedNode, currentView);
                colorizeBranchAction(selectedNode, currentView, patternId);
                t.Commit();
            }
        }

        private void IsolateElements(IGraphNode node, View view)
        {
            view.TemporaryViewModes?.DeactivateMode(TemporaryViewMode.TemporaryHideIsolate);
            List<string> elementIds = node?.ElementIds ?? new List<string>();
            var sectionBoxId = GetSectionBoxId(view);
            if (sectionBoxId != null)
            {
                elementIds.Add(sectionBoxId);
            }
            view.IsolateElementsTemporary(StringsToElementIds(elementIds));
        }

        private void ColorizeSubbranchElements(IGraphNode node, View view, ElementId patternId)
        {
            if (node == null) return;
            foreach (var subBranch in node.SubBranches)
            {
                ColorizeBranchElements(subBranch, view, patternId);
            }
        }

        private void ColorizeBottomBranchElements(IGraphNode node, View view, ElementId patternId)
        {
            if (node == null) return;
            ColorizeBranchElements(node, view, patternId);
            if (node.SubBranches.Count == 0)
            {
                ColorizeBranchElements(node, view, patternId);
            }
            foreach (var subBranch in node.SubBranches)
            {
                if (subBranch.SubBranches.Count == 0)
                {
                    ColorizeBranchElements(subBranch, view, patternId);
                }
                else
                {
                    ColorizeBottomBranchElements(subBranch, view, patternId);
                }
            }
        }

        private void ColorizeBranchElements(IGraphNode node, View view, ElementId patternId)
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

        /// <summary>
        /// The section box id is the view with id '-1'
        /// see: https://forums.autodesk.com/t5/revit-api-forum/get-section-box-of-3d-view/td-p/9795973
        /// </summary>
        private string GetSectionBoxId(View view)
        {
            if (view is View3D view3D && view3D.IsSectionBoxActive)
            {
                var sid = view3D.Id.IntegerValue - 1;
                return sid.ToString();
            }
            return null;
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
