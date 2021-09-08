using System;
using UnityEngine;
using Verse;

namespace RangedDPS.GUIUtils
{
    /// <summary>
    /// Extension methods for working with Rects
    /// </summary>
    public static class RectUtils
    {
        /// <summary>
        /// Creates a new Rect with the given margin on all sides
        /// </summary>
        /// <param name="rect">Rect.</param>
        /// <param name="margin">Margin.</param>
        public static Rect Margin(this Rect rect, float margin)
        {
            return rect.ContractedBy(margin);
        }

        /// <summary>
        /// Creates a new Rect with the given margins
        /// </summary>
        /// <returns>The margins.</returns>
        /// <param name="rect">Rect.</param>
        /// <param name="topMargin">Top margin.</param>
        /// <param name="bottomMargin">Bottom margin.</param>
        /// <param name="leftMargin">Left margin.</param>
        /// <param name="rightMargin">Right margin.</param>
        public static Rect Margins(this Rect rect, float topMargin = 0f, float bottomMargin = 0f, float leftMargin = 0f, float rightMargin = 0f)
        {
            return new Rect {
                yMin = rect.yMin + topMargin,
                yMax = rect.yMax - bottomMargin,
                xMin = rect.xMin + leftMargin,
                xMax = rect.xMax - rightMargin,
            };
        }
    }
}
