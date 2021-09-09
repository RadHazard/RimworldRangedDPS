using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace RangedDPS.GUIUtils
{
    /// <summary>
    /// Scrollable list widget that renders a virtual list of entries.
    /// Only entries that are visible in the window will be rendered for performance reasons.
    /// </summary>
    public abstract class ScrollableList<T>
    {
        private readonly IReadOnlyList<T> entries;

        private Vector2 scrollPosition;

        protected abstract float EntryHeight { get; }

        protected ScrollableList(List<T> entries)
        {
            this.entries = entries;
        }

        /// <summary>
        /// Draw the full list
        /// </summary>
        /// <param name="inRect">In rect.</param>
        /// <param name="selectedEntries">(Optional) the entries that are currently selected, if any</param>
        public void Draw(in Rect inRect, IEnumerable<T> selectedEntries = null)
        {
            HashSet<T> selected = new HashSet<T>(selectedEntries ?? Enumerable.Empty<T>());

            // Cache these for performance reasons in case the underlying calls are expensive
            // (they shouldn't be changing in the middle of a draw operation anyway)
            float entryHeight = EntryHeight;

            Rect viewRect = new Rect
            {
                width = inRect.width,
                height = entryHeight * entries.Count
            };

            int firstVisibleEntry = Mathf.Clamp(Mathf.FloorToInt(scrollPosition.y / entryHeight), 0, entries.Count - 1);
            int lastVisibleEntry = Mathf.Clamp(Mathf.CeilToInt((scrollPosition.y + inRect.height) / entryHeight) + 1, 0, entries.Count - 1);

            Widgets.BeginScrollView(inRect, ref scrollPosition, viewRect, true);
            GUI.BeginGroup(viewRect);
            try
            {
                for (int i = firstVisibleEntry; i <= lastVisibleEntry; i++)
                {
                    T entry = entries[i];
                    DrawEntry(entry, new Rect(0, i * entryHeight, viewRect.width, entryHeight), selected.Contains(entry));
                }
            }
            finally
            {
                GUI.EndGroup();
                Widgets.EndScrollView();
            }
        }

        /// <summary>
        /// Draws a single entry in the list
        /// </summary>
        /// <param name="entry">The entry to draw</param>
        /// <param name="inRect">In rect.</param>
        /// <param name="selected">Whether the entry is selected</param>
        protected abstract void DrawEntry(T entry, in Rect inRect, bool selected);
    }
}
