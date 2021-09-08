using System;
using RangedDPS.GUIUtils;
using UnityEngine;
using Verse;

namespace RangedDPS.CompareTool
{
    /// <summary>
    /// A comparison tab for the compare tool
    /// </summary>
    public abstract class Tab
    {
        [TweakValue("___RangedDPS", 0, 1)]
        private static float GraphWidthPercent = 0.7f;

        [TweakValue("___RangedDPS", 0, 1)]
        private static float GraphHeightPercent = 0.7f;

        [TweakValue("___RangedDPS", 0, 20)]
        private static float Margin = 6f;

        public abstract string Label { get; }

        protected readonly LineGraph graph = new LineGraph();

        /// <summary>
        /// Draws the tab contents
        /// </summary>
        /// <param name="inRect">The rectangle to draw within.</param>
        public void DoTabContents(Rect inRect)
        {
            inRect.SplitVertically(GraphWidthPercent * inRect.width, out Rect graphRect, out Rect menuRect);

            DoGraphContents(graphRect);
            DoMenuContents(menuRect);
        }

        /// <summary>
        /// Draws the graph area
        /// </summary>
        /// <param name="inRect">The rectangle to draw within.</param>
        protected void DoGraphContents(Rect inRect)
        {
            Rect content = inRect.ContractedBy(Margin);
            content.SplitHorizontally(GraphHeightPercent * content.height, out Rect graphRect, out Rect legendRect);

            graph.Draw(graphRect, legendRect);
        }

        /// <summary>
        /// Draws the selection menu
        /// </summary>
        /// <param name="inRect">The rectangle to draw within.</param>
        protected abstract void DoMenuContents(Rect inRect);
    }
}
