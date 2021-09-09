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

        /// <summary>
        /// Splits a rectangle into three parts with a fixed left and right width.
        /// </summary>
        /// <returns>The three sub-rects.</returns>
        /// <param name="rect">Rect.</param>
        /// <param name="leftWidth">Width of the left rect, in pixels.</param>
        /// <param name="rightWidth">Width of the right rect, in pixels.</param>
        /// <param name="margin">(Optional) margin between the subrects</param>
        public static (Rect left, Rect center, Rect right) SplitVerticallyThreeWay(this Rect rect, float leftWidth, float rightWidth, float margin = 0f)
        {
            rect.SplitVertically(leftWidth, out Rect left, out Rect remainder, margin);
            remainder.SplitVertically(remainder.width - rightWidth, out Rect center, out Rect right, margin);
            return (left, center, right);
        }

        /// <summary>
        /// Splits a rectangle into three parts with a fixed top and bottom width.
        /// </summary>
        /// <returns>The three sub-rects.</returns>
        /// <param name="rect">Rect.</param>
        /// <param name="topHeight">Width of the top rect, in pixels.</param>
        /// <param name="bottomHeight">Width of the bottom rect, in pixels.</param>
        /// <param name="margin">(Optional) margin between the subrects</param>
        public static (Rect top, Rect center, Rect bottom) SplitHorizontallyThreeWay(this Rect rect, float topHeight, float bottomHeight, float margin = 0f)
        {
            rect.SplitHorizontally(topHeight, out Rect top, out Rect remainder, margin);
            remainder.SplitHorizontally(remainder.height - bottomHeight, out Rect center, out Rect bottom, margin);
            return (top, center, bottom);
        }
    }
}
