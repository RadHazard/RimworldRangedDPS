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

        protected readonly LineGraph graph = new LineGraph();

        /// <summary>
        /// Draws the tab contents
        /// </summary>
        /// <param name="inRect">The rectangle to draw within.</param>
        public void DoTabContents(Rect inRect)
        {
            GUI.BeginGroup(inRect);

            float graphWidth = inRect.width * GraphWidthPercent;
            float graphHeight = inRect.height * GraphHeightPercent;

            float menuWidth = inRect.width - graphWidth;
            float legendHeight = inRect.height - graphHeight;

            Rect graphRect = new Rect(0f, 0f, graphWidth, graphHeight);
            Rect legendRect = new Rect(0f, graphHeight, inRect.width, legendHeight);

            graph.Draw(graphRect, legendRect);

            GUI.EndGroup();
        }

        /// <summary>
        /// Draws the selection menu
        /// </summary>
        /// <param name="inRect">The rectangle to draw within.</param>
        public abstract void DoMenuContents(Rect inRect);
    }
}
