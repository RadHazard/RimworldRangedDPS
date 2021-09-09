using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RangedDPS.GUIUtils
{
    /// <summary>
    /// A scrollable list for selecting one or more pawns
    /// </summary>
    public class ScrollableList_PawnSelect : ScrollableList<Pawn>
    {
        private readonly Action<Pawn> selectPawn;

        [TweakValue("___RangedDPS", 0f, 100f)]
        private static float SelectPawnEntryHeight = 35f;

        [TweakValue("___RangedDPS", 0f, 200f)]
        private static float SelectPawnAcceptWidth = 70f;

        [TweakValue("___RangedDPS", 0f, 10f)]
        private static float SelectPawnIconMargin = 3f;

        protected override float EntryHeight => SelectPawnEntryHeight;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:RangedDPS.GUIUtils.ScrollableList_PawnSelect"/> class.
        /// </summary>
        /// <param name="selectPawn">The action to run when selecting a pawn.</param>
        /// <param name="pawns">The list of pawns.</param>
        public ScrollableList_PawnSelect(Action<Pawn> selectPawn, List<Pawn> pawns) : base(pawns)
        {
            this.selectPawn = selectPawn;
        }

        /// <summary>
        /// Draws an entry.
        /// </summary>
        /// <param name="entry">Entry.</param>
        /// <param name="inRect">In rect.</param>
        /// <param name="selected">Whether the entry is s</param>
        protected override void DrawEntry(Pawn entry, in Rect inRect, bool selected)
        {
            if (selected)
                Widgets.DrawHighlightSelected(inRect);

            (Rect icon, Rect name, Rect button) = inRect.SplitVerticallyThreeWay(SelectPawnEntryHeight, SelectPawnAcceptWidth);

            Widgets.ThingIcon(icon.Margin(SelectPawnIconMargin), entry);
            Widgets.Label(name, entry.LabelCap);

            //TODO click for infocard

            if (!selected)
            {
                if (Widgets.ButtonText(button, "Select")) // TODO translate
                {
                    selectPawn(entry);
                }
            }

            //var oldFont = Text.Font;
            //Text.Font = GameFont.Small;

            //Text.Font = oldFont;
        }
    }
}
