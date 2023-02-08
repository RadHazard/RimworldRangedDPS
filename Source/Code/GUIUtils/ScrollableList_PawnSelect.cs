using System;
using System.Collections.Generic;
using JetBrains.Annotations;
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
        [UsedImplicitly]
        private static float PawnEntryHeight = 35f;

        [TweakValue("___RangedDPS", 0f, 200f)]
        [UsedImplicitly]
        private static float AcceptWidth = 70f;

        [TweakValue("___RangedDPS", 0f, 10f)]
        [UsedImplicitly]
        private static float SelectPawnIconMargin = 3f;

        protected override float EntryHeight => PawnEntryHeight;

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

            var (icon, name, button) = inRect.SplitVerticallyThreeWay(PawnEntryHeight, AcceptWidth);

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
